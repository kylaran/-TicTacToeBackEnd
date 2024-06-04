using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Xml;
using TicTakToe.Attributs;
using TicTakToe.Context;
using TicTakToe.Entity.BaseModels;
using TicTakToe.Entity.Models.Dto;
using TicTakToe.Entity.Models.Vm;
using TicTakToe.Entity.Models.Vm.SSE;
using TicTakToe.Infrastructure;

namespace TicTakToe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SSEController : ControllerBase
    {

        private readonly ILogger<SSEController> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SSEController(ILogger<SSEController> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // Метод для SSE
        /// <summary>
        /// SSE Request
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("updates/{gameId}/{userId}")]
        [NoTimeout]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task Updates(int gameId, int userId)
        {
            Response.Headers["Content-Type"] = "text/event-stream";

            bool keepConnectionAlive = true;
            while (keepConnectionAlive)
            {
                SSEResponseMethodVm update;

                // Создание нового соединения с базой данных
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
                    update = await GetGameUpdate(context, gameId, userId);
                }

                if (!update.Seen)
                {
                    if (update.Status != SSEStatusEnum.Nothing)
                    {
                        var data = $"data: {JsonSerializer.Serialize(new SSEResponseVm(update))}\n\n";
                        var bytes = Encoding.UTF8.GetBytes(data);
                        await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                        await Response.Body.FlushAsync();
                    }
                }
                // Условие для завершения соединения
                if (update.NeedClose)
                {
                    keepConnectionAlive = false;
                }

                await Task.Delay(1000); // Пауза перед следующей проверкой
            }

            // Завершение соединения
            Response.Body.Close();
        }

        private async Task<SSEResponseMethodVm> GetGameUpdate(TicTacToeDbContext context, int gameId, int userId)
        {
            var result = new SSEResponseMethodVm();
            var game = await context.Games
                .Include(x => x.Table)
                .FirstOrDefaultAsync(x => x.Id == gameId);

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.IdTg == userId);

            if (game != null)
            {
                if ((DateTime.UtcNow - game.LastMoveDate).TotalMinutes > 5)
                {
                    result.Status = SSEStatusEnum.Nothing;
                    result.NeedClose = true;
                }
                else
                if (user.SeenChanges)
                {
                    result.Seen = true;
                    result.Status = SSEStatusEnum.Nothing;
                }
                else
                {
                    if (game.LastMoveFrom == userId)
                    {
                        if (game.Status == StatusGameEnum.WaitingOpponent
                            || game.Status == StatusGameEnum.GameOn)
                        {
                            result.Status = SSEStatusEnum.Nothing;
                        }
                        if (game.Status == StatusGameEnum.GameOverDraw)
                        {
                            user.SeenChanges = true;
                            result.NeedClose = true;
                            result.Status = SSEStatusEnum.Over;
                            result.Data = new OverDataVm
                            {
                                statusGame = game.Status
                            };
                        }
                    }
                    else
                    {
                        var isCellsNull = game.Table.Cells.All(c => c == CellEnum.None);
                        if (game.Status == StatusGameEnum.GameOn && isCellsNull)
                        {
                            game.LastMoveFromSee = true;
                            result.Status = SSEStatusEnum.Joined;
                            result.Data = game.FirstStep;
                        }
                        if (game.Status == StatusGameEnum.GameOn && !isCellsNull)
                        {
                            game.LastMoveFromSee = true;
                            result.Status = SSEStatusEnum.Move;
                            result.Data = game.LastMoveIndex;
                        }
                        else if (game.Status == StatusGameEnum.GameOverGaveUp
                            || game.Status == StatusGameEnum.GameOverDraw)
                        {
                            game.LastMoveFromSee = true;
                            result.Status = SSEStatusEnum.Over;
                            result.NeedClose = true;
                            result.Data = new OverDataVm
                            {
                                whoWin = game.Status == StatusGameEnum.GameOverDraw ? null : userId,
                                statusGame = game.Status,
                                lastMove = game.LastMoveIndex.HasValue ? game.LastMoveIndex.Value : 0,
                            };
                        }
                        else if (
                             game.Status == StatusGameEnum.GameOver)
                        {
                            game.LastMoveFromSee = true;
                            result.NeedClose = true;
                            result.Status = SSEStatusEnum.Over;
                            result.Data = new OverDataVm
                            {
                                whoWin = game.LastMoveFrom == userId ?
                                userId : game.LastMoveFrom,

                                statusGame = game.Status,
                                lastMove = game.LastMoveIndex.HasValue ? game.LastMoveIndex.Value : 0,
                            };
                        }
                        else
                        {
                            if (game.Status == StatusGameEnum.WaitingOpponent)
                            {
                                result.Status = SSEStatusEnum.Nothing;
                            }
                        }
                        result.Seen = false;
                        user.SeenChanges = true;
                        context.Users.Update(user);
                        context.Games.Update(game);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                result.Status = SSEStatusEnum.Nothing;
                result.NeedClose = true;
            }
            return result;
        }
    }
}
