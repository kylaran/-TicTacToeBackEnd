using Microsoft.EntityFrameworkCore;
using Quartz;
using TicTakToe.Context;
using TicTakToe.Entity.BaseModels;
using TicTakToe.Entity.Models.Jobs;

namespace TicTakToe.Jobs
{
    public class CloseExistGamesJob: IJob
    {
        private readonly ILogger<CloseExistGamesJob> _logger;
        private readonly TicTacToeDbContext _context;

        public CloseExistGamesJob(
            ILogger<CloseExistGamesJob> logger,
            TicTacToeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// /Удаление логов раз в 3 дня
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var gameForClose = new List<Game>();

                var notClosedGames = _context.Games
                 .Include(x => x.Table)
                 .Where(x =>
                 x.Status != StatusGameEnum.GameClosed ||
                 x.Status != StatusGameEnum.GameOver ||
                 x.Status != StatusGameEnum.GameFailed ||
                 x.Status != StatusGameEnum.GameOverGaveUp ||
                 x.Status != StatusGameEnum.GameOverDraw)
                 .AsQueryable();

                var waitingGamesOpened = notClosedGames
                    .Where(x => x.Status == StatusGameEnum.WaitingOpponent)
                    .AsQueryable();

                var gameOnOpened = notClosedGames
                    .Where(x => x.Status == StatusGameEnum.GameOn)
                    .AsQueryable();

                waitingGamesOpened = waitingGamesOpened
                    .Where(x => (DateTime.UtcNow - x.LastMoveDate).TotalMinutes > 1);

                gameOnOpened = gameOnOpened
                    .Where(x => ((DateTime.UtcNow - x.LastMoveDate).TotalSeconds + 10) > x.MoveTime);
            
                if (waitingGamesOpened.Any() )
                {
                    foreach( var game in waitingGamesOpened)
                    {
                        gameForClose.Add(game);
                    }
                }
                if (gameOnOpened.Any() )
                {
                    foreach(var game in gameOnOpened)
                    {
                        gameForClose.Add(game);
                    }
                }

                if (gameForClose.Any())
                {
                    var usersUpdate = new List<User>();

                    foreach( var game in gameForClose)
                    {
                        var usersMovment = await ReturnMoney(game);
                        usersUpdate = ChangeMoney(usersUpdate, usersMovment);

                        game.Status = StatusGameEnum.GameFailed;
                    }
                    _context.Users.UpdateRange(usersUpdate);
                    _context.Games.UpdateRange(gameForClose);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseExistGamesJob error");
            }
        }

        private async Task<List<ReturnMoneyObj>> ReturnMoney(Game game)
        {
            var result = new List<ReturnMoneyObj>();

            var creater = await _context.Users
                .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);
            if (game.ParticipantJoined != null)
            {
                if (game.ParticipantJoined != 0)
                {
                    var joiner = await _context.Users
                        .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);

                    result.Add(new ReturnMoneyObj
                    {
                        Movment = game.PriceGame / 2,
                        User = joiner
                    });
                    result.Add(new ReturnMoneyObj
                    {
                        Movment = game.PriceGame / 2,
                        User = creater
                    });
                }
            }
            else
            {

                result.Add(new ReturnMoneyObj
                {
                    Movment = game.PriceGame,
                    User = creater
                });
            }
            return result;
        }

        private List<User> ChangeMoney(List<User> fullList, List<ReturnMoneyObj> newList)
        {
            foreach (var item in newList)
            {
                var user = fullList
                    .FirstOrDefault(u => u.IdTg == item.User.IdTg);
               
                if (user == null)
                {
                    user = item.User;
                    user.FreeCoin += item.Movment;

                    fullList.Add(user);
                }
                else
                {
                    user.FreeCoin += item.Movment;
                }
            }
            return fullList;
        }
    }
}
