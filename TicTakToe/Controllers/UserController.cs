using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TicTakToe.Context;
using TicTakToe.Entity.BaseModels;
using TicTakToe.Entity.Models.Dto;
using TicTakToe.Entity.Models.Vm;

namespace TicTakToe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static string TokenBot = "7236432554:AAEpA__XRS0kvDViHJ_H0wc1ztKFGEH4VMM";
        private readonly ILogger<UserController> _logger;

        private TicTacToeDbContext _context;

        public UserController(ILogger<UserController> logger, TicTacToeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Request in app component for init
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserVm), StatusCodes.Status200OK)]
        public async Task<UserVm> userLogin([FromBody] UserDto user)
        {
            try
            {
                var checkBot = await _context.Users
                .FirstOrDefaultAsync(x => x.IdTg == 0 && x.Is_bot == true);

                var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.IdTg == user.Id);
                if (checkUser != null)
                {
                    if (checkUser.Photo_url == null)
                    {
                        var photo = await GetPhoto(user.Id);
                        if (photo != null)
                        {
                            checkUser.Photo_url = photo;
                            checkUser.First_name = user.first_name;
                            checkUser.Last_name = user.last_name;
                            checkUser.Username = user.username;
                            checkUser.SeenChanges = false;
                            _context.Update(checkUser);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            checkUser.First_name = user.first_name;
                            checkUser.Last_name = user.last_name;
                            checkUser.Username = user.username;
                            checkUser.SeenChanges = false;
                            _context.Update(checkUser);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var photo = await GetPhoto(user.Id);
                        if (photo != null)
                        {
                            checkUser.Photo_url = photo;
                            checkUser.First_name = user.first_name;
                            checkUser.Last_name = user.last_name;
                            checkUser.Username = user.username;
                            checkUser.SeenChanges = false;
                            _context.Update(checkUser);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            checkUser.First_name = user.first_name;
                            checkUser.Last_name = user.last_name;
                            checkUser.Username = user.username;
                            checkUser.SeenChanges = false;
                            _context.Update(checkUser);
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (checkBot == null)
                    {
                        var createBot = new User
                        {
                            IdTg = 0,
                            Is_bot = true,
                            First_name = "Bot",
                            Photo_url = "https://i.ibb.co/G9Dz245/profile-Bot.jpg",
                            FreeCoin = 0,
                            TonCoin = 0
                        };
                        _context.Users.Add(createBot);
                        await _context.SaveChangesAsync();
                    }
                    return new UserVm(checkUser);
                }
                else
                {

                    var newUser = new User(user);
                    if (checkBot == null)
                    {
                        var createBot = new User
                        {
                            IdTg = 0,
                            Is_bot = true,
                            First_name = "Bot",
                            Photo_url = "https://i.ibb.co/G9Dz245/profile-Bot.jpg",
                            FreeCoin = 0,
                            TonCoin = 0
                        };
                        _context.Users.AddRange([createBot, newUser]);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();
                    }
                    return new UserVm(newUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public async Task<UserVm> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IdTg == id);
                return new UserVm(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get user balance by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("balance/{id}")]
        public async Task<UserBalanceVm> GetMyBalance(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IdTg == id);
                return new UserBalanceVm(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [HttpPost, Route("coin")]
        [ProducesResponseType(typeof(UserVm), StatusCodes.Status200OK)]
        public async Task<UserVm> userLogin([FromBody] AddBalanceDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IdTg == dto.UserId);
                if (user != null)
                {
                    user.FreeCoin += dto.FreeCoin;
                    user.LastGiftReceived = DateTime.UtcNow;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                return new UserVm(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get history games for user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("history")]
        [ProducesResponseType(typeof(GameHistoryVm), StatusCodes.Status200OK)]
        public async Task<GameHistoryVm> userLogin([FromQuery] GameHistorySearchDto dto)
        {
            try
            {
                var result = new GameHistoryVm
                {
                    TotalCount = 0,
                    Take = 0,
                    Skip = 0,
                    Data = new List<GameHistory>()
                };

                var history = _context.History
                    .Where(x => x.UserId == dto.UserId)
                    .OrderByDescending(x => x.FinishedDate)
                    .AsQueryable();

                if (history.Any())
                {
                    result.TotalCount = history.Count();
                    result.Take = dto.Take;
                    result.Skip = dto.Skip;
                    result.Data = await history
                                            .Take(dto.Take)
                                            .Skip(dto.Skip)
                                            .ToListAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        private async Task<string?> GetPhoto(int userId)
        {
            var bot = new TelegramBotClient(TokenBot);
            var photos = await bot.GetUserProfilePhotosAsync(userId);
            if (photos.TotalCount > 0)
            {
                var photo = photos.Photos.FirstOrDefault()[0];
                if (photo != null)
                {
                    var file = await bot.GetFileAsync(photo.FileId);
                    using (var memoryStream = new MemoryStream())
                    {
                        await bot.DownloadFileAsync(file.FilePath, memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(fileBytes);
                    }
                }
                else return null;
            }
            else return null;
        }
    }
}
