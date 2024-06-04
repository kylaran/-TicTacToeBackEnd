using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTakToe.Context;
using TicTakToe.Entity.BaseModels;
using TicTakToe.Entity.Models.Dto;
using TicTakToe.Entity.Models.Vm;
using TicTakToe.Infrastructure;

namespace TicTakToe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {

        private readonly ILogger<GameController> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private TicTacToeDbContext _context;
        public GameController(ILogger<GameController> logger, TicTacToeDbContext context, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _context = context;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Request for create new game
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> CreateGame([FromBody] CreateGameDto dto)
        {
            try
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(x => x.IdTg == dto.UserId);

                if (user == null)
                    return new GameVm();

                var date = DateTime.UtcNow;

                if (dto.WithBot)
                    dto.Price = 0;

                if (user.FreeCoin < dto.Price)
                {
                    return new GameVm();
                }

                var game = new Game
                {
                    CreatedDate = date,
                    ParticipantCreater = dto.UserId,
                    IsFreeGame = dto.IsFreeGame,
                    Status = StatusGameEnum.WaitingOpponent,
                    LastMoveFrom = dto.UserId,
                    LastMoveDate = date,
                    MoveTime = dto.MoveTime,
                    IsPrivateGame = dto.IsPrivateGame,
                    PriceGame = dto.Price,
                    FirstStep = dto.FirstStep,
                    Table = new Table
                    {
                        WallSize = dto.WallSize,
                        WinSize = dto.WinSize,
                        Cells = GenerateCells(dto.WallSize),
                    }
                };

                user.SeenChanges = false;
                user.FreeCoin -= dto.Price;
                if (dto.WithBot)
                {
                    var _bot = new Bot();
                    game.WithBot = true;

                    game.ParticipantJoined = 0;
                    if (game.FirstStep == FirstStepEnum.Random)
                    {
                        Random random = new Random();
                        if (random.Next(1, 3) == 1)
                        {
                            game.FirstStep = FirstStepEnum.Creater;
                        }
                        else
                        {

                            game.FirstStep = FirstStepEnum.Joiner;
                            var botMove = _bot.MakeMove(game, 0, game.FirstStep == FirstStepEnum.Creater ? CellEnum.Tac : CellEnum.Tic);
                            game.LastMoveDate = date;
                            game.LastMoveFrom = botMove.UserId;
                            game.LastMoveIndex = botMove.NumberCell;
                            game.Status = StatusGameEnum.GameOn;
                            game.Table.Cells[botMove.NumberCell] = botMove.Cell;
                        }
                    }
                    else if (game.FirstStep == FirstStepEnum.Joiner)
                    {
                        game.FirstStep = FirstStepEnum.Joiner;
                        var botMove = _bot.MakeMove(game, 0, game.FirstStep == FirstStepEnum.Creater ? CellEnum.Tac : CellEnum.Tic);
                        game.LastMoveDate = date;
                        game.LastMoveFrom = botMove.UserId;
                        game.LastMoveIndex = botMove.NumberCell;
                        game.Status = StatusGameEnum.GameOn;
                        game.Table.Cells[botMove.NumberCell] = botMove.Cell;
                    }
                }

                _context.Users.Update(user);
                _context.Games.Add(game);
                await _context.SaveChangesAsync();


                var createdGame = await _context.Games
                    .FirstOrDefaultAsync(x =>
                    x.CreatedDate == date &&
                    x.LastMoveDate == date &&
                    x.ParticipantCreater == dto.UserId
                );

                var participant = new ParticipantVm(user);
                if (dto.WithBot)
                {
                    var bot = await _context.Users.FirstOrDefaultAsync(x =>
                    x.IdTg == 0);

                    await Task.Delay(1000);

                    var participantJoined = new ParticipantVm(bot);
                    return new GameVm(createdGame, participant, participantJoined);
                }
                else
                {
                    return new GameVm(createdGame, participant, null);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Request for join to game
        /// </summary>
        /// <param name="dto"></param>
        /// <response code="200">game object</response>
        [HttpPost, Route("join")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> JoinGame([FromBody] JoinGameDto dto)
        {
            try
            {
                var userJoin = await _context.Users
                .FirstOrDefaultAsync(x => x.IdTg == dto.UserId);

                userJoin.SeenChanges = false;

                var game = await _context.Games
                .Include(t => t.Table)
                .FirstOrDefaultAsync(x => x.Id == dto.GameId);

                if (userJoin.FreeCoin < game.PriceGame)
                {
                    return new GameVm();
                }

                if (game.FirstStep == FirstStepEnum.Random)
                {
                    Random random = new Random();
                    if (random.Next(1, 3) == 1)
                    {
                        game.FirstStep = FirstStepEnum.Creater;
                    }
                    else game.FirstStep = FirstStepEnum.Joiner;
                }
                userJoin.FreeCoin -= game.PriceGame;
                game.PriceGame *= 2;
                game.Status = StatusGameEnum.GameOn;
                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = userJoin.IdTg.Value;
                game.ParticipantJoined = dto.UserId;
                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);

                _context.Users.Update(userJoin);
                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                var participantCreater = new ParticipantVm(userCreater);
                var participantJoined = new ParticipantVm(userJoin);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BotWin Request Exception");
                return null;
            }
        }

        /// <summary>
        /// Request for move in game, and send if move for win, or for draw
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("move")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> MoveInGame([FromBody] MoveGameDto dto)
        {
            try
            {
                var game = await _context.Games
                    .Include(x => x.Table)
                    .FirstOrDefaultAsync(x => x.Id == dto.GameId);
                var isFirstMove = game.Table.Cells.All(x => x == CellEnum.None);

                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = dto.UserId;
                game.Table.Cells[dto.NumberCell] = dto.Cell;
                game.LastMoveIndex = dto.NumberCell;
                game.LastMoveFromSee = false;

                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);
                var userJoin = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);

                if (isFirstMove)
                {
                    userCreater.SeenChanges = false;
                    userJoin.SeenChanges = false;
                }
                else
                {
                    if (userCreater.IdTg == dto.UserId)
                    {
                        userCreater.SeenChanges = false;
                    }
                    else
                    {
                        userJoin.SeenChanges = false;
                    }
                }

                if (dto.IsMoveWin)
                {
                    var historyForSave = new List<GameHistory>();
                    game.Status = StatusGameEnum.GameOver;
                    if (userCreater.IdTg == dto.UserId)
                    {
                        var history = await AddHistory(userCreater, userJoin, game, true);

                        if (history != null)
                            historyForSave.Add(history);

                        history = null;
                        history = await AddHistory(userJoin, userCreater, game, false);

                        if (history != null)
                            historyForSave.Add(history);

                        userCreater.FreeCoin += game.PriceGame;
                        _context.History.AddRange(historyForSave);
                        _context.Users.Update(userCreater);
                    }
                    else
                    {
                        var history = await AddHistory(userCreater, userJoin, game, false);
                        if (history != null)
                            historyForSave.Add(history);

                        history = null;
                        history = await AddHistory(userJoin, userCreater, game, true);
                        if (history != null)
                            historyForSave.Add(history);

                        userJoin.FreeCoin += game.PriceGame;
                        _context.History.AddRange(historyForSave);
                        _context.Users.Update(userJoin);
                    }
                }
                if (dto.isDrawGame)
                {
                    var historyForSave = new List<GameHistory>();

                    var history = await AddHistory(userCreater, userJoin, game, true, true);
                    if (history != null)
                        historyForSave.Add(history);

                    history = null;
                    history = await AddHistory(userJoin, userCreater, game, true, true);
                    if (history != null)
                        historyForSave.Add(history);

                    game.Status = StatusGameEnum.GameOverDraw;
                    userCreater.FreeCoin += game.PriceGame / 2;
                    userJoin.FreeCoin += game.PriceGame / 2;

                }
                _context.Update(userCreater);
                _context.Update(userJoin);
                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                await Task.Delay(1250);

                var participantCreater = new ParticipantVm(userCreater);
                var participantJoined = new ParticipantVm(userJoin);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BotWin Request Exception");
                return null;
            }
        }

        /// <summary>
        /// Request for move in game with bot
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("move/bot")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> MoveInGameWithBot([FromBody] MoveGameDto dto)
        {
            try
            {
                var _bot = new Bot();
                var game = await _context.Games
                    .Include(x => x.Table)
                    .FirstOrDefaultAsync(x => x.Id == dto.GameId);

                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = dto.UserId;
                game.Table.Cells[dto.NumberCell] = dto.Cell;
                game.LastMoveFromSee = false;
                game.Status = StatusGameEnum.GameOn;

                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);
                var userJoin = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);

                if (dto.IsMoveWin == false && dto.isDrawGame == false)
                {
                    var botMove = _bot.MakeMove(game, 0, game.FirstStep == FirstStepEnum.Creater ? CellEnum.Tac : CellEnum.Tic);

                    game.LastMoveDate = DateTime.UtcNow;
                    game.LastMoveFrom = botMove.UserId;
                    game.LastMoveIndex = botMove.NumberCell;

                    game.Table.Cells[botMove.NumberCell] = botMove.Cell;

                }

                if (dto.IsMoveWin)
                {
                    game.Status = StatusGameEnum.GameOver;

                    var history = await AddHistory(userJoin, userCreater, game, true);
                    if (history != null)
                    {
                        _context.History.Update(history);
                    }
                }
                if (dto.isDrawGame)
                {
                    game.Status = StatusGameEnum.GameOverDraw;
                }

                if (dto.isBotWin.HasValue && dto.isBotWin.Value == true)
                {
                    game.Status = StatusGameEnum.GameOver;
                    var history = await AddHistory(userCreater, userJoin, game, false);
                    if (history != null)
                    {
                        _context.History.Update(history);
                    }
                }

                userCreater.SeenChanges = false;
                _context.Users.Update(userCreater);
                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                await Task.Delay(1000);

                var participantCreater = new ParticipantVm(userCreater);
                var participantJoined = new ParticipantVm(userJoin);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BotWin Request Exception");
                return null;
            }
        }

        /// <summary>
        /// Request for move in game with bot
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("move/win")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> BotWinInGame([FromBody] BotWinDto dto)
        {
            try
            {
                var _bot = new Bot();
                var game = await _context.Games
                    .Include(x => x.Table)
                    .FirstOrDefaultAsync(x => x.Id == dto.GameId);

                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = 0;
                game.LastMoveFromSee = false;
                game.Status = StatusGameEnum.GameOn;

                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);
                var userJoin = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);

                game.Status = dto.Status;
                var history = await AddHistory(userCreater, userJoin, game, false);
                if (history != null)
                    _context.History.Update(history);

                userCreater.SeenChanges = false;
                _context.Users.Update(userCreater);
                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                await Task.Delay(1000);

                var participantCreater = new ParticipantVm(userCreater);
                var participantJoined = new ParticipantVm(userJoin);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BotWin Request Exception");
                return null;
            }
        }

        /// <summary>
        /// request if time over
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("failed")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> FinishGame([FromBody] FinishGameDto dto)
        {
            try
            {
                var game = await _context.Games
                    .Include(x => x.Table)
                    .FirstOrDefaultAsync(x => x.Id == dto.GameId);

                game.Status = StatusGameEnum.GameFailed;
                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = dto.UserId;

                var userIdWin = game.ParticipantCreater == dto.UserId ? dto.UserId : game.ParticipantJoined;
                var userIdLose = dto.UserId;

                var userWinner = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == userIdWin);
                var userLoser = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == userIdLose);

                if (game.ParticipantJoined != 0)
                {
                    var historyToSave = new List<GameHistory>();

                    userWinner.FreeCoin += game.PriceGame / 2;
                    userLoser.FreeCoin -= game.PriceGame / 2;

                    var history = await AddHistory(userWinner, userLoser, game, true);
                    if (history != null)
                    {
                        historyToSave.Add(history);
                    }
                    history = null;

                    history = await AddHistory(userLoser, userWinner, game, false);
                    if (history != null)
                    {
                        historyToSave.Add(history);
                    }
                    _context.History.UpdateRange(historyToSave);
                    _context.Users.Update(userWinner);
                    _context.Users.Update(userLoser);
                }

                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                var participantCreater = new ParticipantVm(userIdWin == game.ParticipantCreater ? userWinner : userLoser);
                var participantJoined = new ParticipantVm(participantCreater.UserId == userIdWin ? userLoser : userWinner);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// request if time over
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("giveup")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> GiveUpInGame([FromBody] FinishGameDto dto)
        {

            try
            {
                var historyForSave = new List<GameHistory>();
                var game = await _context.Games
                .Include(x => x.Table)
                .FirstOrDefaultAsync(x => x.Id == dto.GameId);

                game.Status = StatusGameEnum.GameOverGaveUp;
                game.LastMoveDate = DateTime.UtcNow;
                game.LastMoveFrom = dto.UserId;

                var userIdWin = game.ParticipantCreater == dto.UserId ? dto.UserId : game.ParticipantJoined;
                var userIdLose = dto.UserId;

                var userWinner = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == userIdWin);
                var userLoser = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == userIdLose);

                if (game.ParticipantJoined != 0)
                {
                    var historyToSave = new List<GameHistory>();

                    var history = await AddHistory(userWinner, userLoser, game, true);
                    if (history != null)
                    {
                        historyToSave.Add(history);
                    }
                    history = null;

                    history = await AddHistory(userLoser, userWinner, game, false);
                    if (history != null)
                    {
                        historyToSave.Add(history);
                    }
                    _context.History.UpdateRange(historyToSave);

                    userWinner.FreeCoin += game.PriceGame / 2;
                    userLoser.FreeCoin -= game.PriceGame / 2;

                    _context.History.UpdateRange(historyToSave);
                    _context.Users.Update(userWinner);
                    _context.Users.Update(userLoser);
                }

                if (historyForSave.Count() > 0)
                    _context.Games.Update(game);
                await _context.SaveChangesAsync();

                var participantCreater = new ParticipantVm(userIdWin == game.ParticipantCreater ? userWinner : userLoser);
                var participantJoined = new ParticipantVm(participantCreater.UserId == userIdWin ? userLoser : userWinner);

                return new GameVm(game, participantCreater, participantJoined);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Request for get game by Id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpGet, Route("{gameId}")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<GameVm> GetGame(int gameId)
        {
            try
            {
                var game = await _context.Games
                        .Include(x => x.Table)
                        .FirstOrDefaultAsync(x => x.Id == gameId);

                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);

                if (game.ParticipantJoined != null)
                {
                    var userJoin = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantJoined);

                    var participantCreater = new ParticipantVm(userCreater);
                    var participantJoined = new ParticipantVm(userJoin);

                    return new GameVm(game, participantCreater, participantJoined);
                }
                else
                {
                    var participantCreater = new ParticipantVm(userCreater);

                    return new GameVm(game, participantCreater, null);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Request for close game by id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpPost, Route("close/{gameId}")]
        [ProducesResponseType(typeof(GameVm), StatusCodes.Status200OK)]
        public async Task<object> CloseGameById(int gameId)
        {
            try
            {
                var game = await _context.Games
                        .Include(x => x.Table)
                        .FirstOrDefaultAsync(x => x.Id == gameId);

                var userCreater = await _context.Users
                    .FirstOrDefaultAsync(x => x.IdTg == game.ParticipantCreater);

                if (game.ParticipantJoined == null)
                {
                    userCreater.FreeCoin += game.PriceGame;
                    game.Status = StatusGameEnum.GameClosed;
                    _context.Users.Update(userCreater);
                    _context.Games.Update(game);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Request for search game
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("list")]
        [ProducesResponseType(typeof(ListGamesVm), StatusCodes.Status200OK)]
        public async Task<ListGamesVm> GetGames([FromQuery] SearchGameDto dto)
        {
            try
            {
                var result = new ListGamesVm();
                result.filter = dto;
                result.TotalCount = 0;
                result.Take = 0;
                result.Skip = 0;
                result.Data = new List<GameInList>();
                var games = _context.Games
                    .Include(x => x.Table)
                    .Where(x => x.Status == StatusGameEnum.WaitingOpponent &&
                                x.Table.WallSize >= dto.WallSizeFrom &&
                                x.Table.WallSize <= dto.WallSizeTo &&
                                x.Table.WinSize >= dto.WinSizeFrom &&
                                x.Table.WinSize <= dto.WinSizeTo &&
                                x.ParticipantJoined == null)
                    .AsQueryable();

                if (games.Count() > 0)
                {
                    result.TotalCount = games.Count();
                    result.Take = dto.Take;
                    result.Skip = dto.Skip;

                    var gamesInList = await games.Take(dto.Take).Skip(dto.Skip).ToListAsync();

                    foreach (var game in gamesInList)
                    {
                        var user = _context.Users.FirstOrDefault(x => x.IdTg == game.ParticipantCreater);
                        result.Data.Add(new GameInList(game, user));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private CellEnum[] GenerateCells(int size)
        {
            var cells = new CellEnum[size * size];
            for (int i = 0; i < cells.Length; i++)
                cells[i] = CellEnum.None;
            return cells;
        }

        private async Task<GameHistory?> AddHistory(User? user, User? userOponent, Game game, bool isWin, bool isDraw = false)
        {
            var balanceGame = game.PriceGame;
            try
            {
                if ((await CheckHistory(user, game) == false))
                {
                    if (isDraw)
                        balanceGame = balanceGame / 2;

                    if (game.Status == StatusGameEnum.GameOver
                        || game.Status == StatusGameEnum.GameOverGaveUp
                        || game.Status == StatusGameEnum.GameOverDraw
                        || game.Status == StatusGameEnum.GameFailed)
                    {
                        var history = new GameHistory
                        {
                            UserId = user.IdTg.Value,
                            FinishedDate = DateTime.UtcNow,
                            FinancialMovment = isWin ? balanceGame : (0 - balanceGame),
                            IsFreeGame = game.IsFreeGame,
                            IsWin = isWin,
                            GameId = game.Id,
                            StatusGame = game.Status,
                            OponentName = userOponent.First_name,
                            OponentPhoto = userOponent.Photo_url
                        };
                        return history;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        private async Task<bool> CheckHistory(User? user, Game game)
        {
            var result = false;
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
                    var check = await context.History
                        .FirstOrDefaultAsync(x => x.GameId == game.Id && x.UserId == user.IdTg);
                    if (check != null)
                    {
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
