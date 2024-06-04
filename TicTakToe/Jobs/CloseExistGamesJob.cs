using Microsoft.EntityFrameworkCore;
using Quartz;
using TicTakToe.Context;
using TicTakToe.Entity.BaseModels;

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
                    foreach( var game in gameForClose)
                    {
                        ReturnMoney(game);
                        game.Status = StatusGameEnum.GameFailed;
                    }
                    _context.UpdateRange(gameForClose);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseExistGamesJob error");
            }
        }

        private async Task<List<User>> ReturnMoney(Game game)
        {
            var usersUpdate = new List<User>();

            var creater = await _context.Users
                .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);
            if (game.ParticipantJoined != null)
            {
                if (game.ParticipantJoined != 0)
                {
                    var joiner = await _context.Users
                        .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);
                    joiner.FreeCoin += game.PriceGame / 2;
                    usersUpdate.Add(joiner);
                    creater.FreeCoin += game.PriceGame / 2;
                    usersUpdate.Add(creater);
                }
            }
            else
            {
                usersUpdate.Add(creater);
                creater.FreeCoin += game.PriceGame;
            }
            return usersUpdate;
        }
    }
}
