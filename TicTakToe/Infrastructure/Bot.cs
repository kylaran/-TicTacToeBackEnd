using TicTakToe.Entity.BaseModels;
using TicTakToe.Entity.Models.Dto;

namespace TicTakToe.Infrastructure
{
    public class Bot
    {
        public MoveGameDto MakeMove(Game game, int userId, CellEnum cellBot)
        {
            // Создаем копию текущего состояния поля
            var cells = (CellEnum[])game.Table.Cells.Clone();

            var cellUser = cellBot == CellEnum.Tac ? CellEnum.Tic : CellEnum.Tac; //чем ходит пользователь

            if (IsFirstMove(cells))
            {
                return MakeFirstMove(game, userId, cellBot, cells);
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == CellEnum.None)
                {
                    cells[i] = cellBot;
                    if (IsWinningMove(cells, game.Table.WinSize, cellBot))
                    {
                        return new MoveGameDto
                        {
                            GameId = game.Id,
                            UserId = userId,
                            NumberCell = i,
                            Cell = cellBot,
                            IsMoveWin = true,
                            isDrawGame = false
                        };
                    }
                    cells[i] = CellEnum.None;
                }
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == CellEnum.None)
                {
                    cells[i] = cellUser;
                    if (IsWinningMove(cells, game.Table.WinSize, cellUser))
                    {
                        return new MoveGameDto
                        {
                            GameId = game.Id,
                            UserId = userId,
                            NumberCell = i,
                            Cell = cellBot,
                            IsMoveWin = false,
                            isDrawGame = false
                        };
                    }
                    cells[i] = CellEnum.None;
                }
            }

            // Если нет критических ходов, выбираем случайную свободную клетку
            var random = new Random();
            int move;
            do
            {
                move = random.Next(0, cells.Length);
            } while (cells[move] != CellEnum.None);

            return new MoveGameDto
            {
                GameId = game.Id,
                UserId = userId,
                NumberCell = move,
                Cell = cellBot,
                IsMoveWin = false,
                isDrawGame = false
            };
        }

        private bool IsWinningMove(CellEnum[] cells, int winSize, CellEnum player)
        {
            // Линии по горизонтали:
            int n = (int)Math.Sqrt(cells.Length);
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col <= n - winSize; col++)
                {
                    bool win = true;
                    for (int k = 0; k < winSize; k++)
                    {
                        if (cells[row * n + col + k] != player)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        return true;
                    }
                }
            }
            // По вертикали:
            for (int col = 0; col < n; col++)
            {
                for (int row = 0; row <= n - winSize; row++)
                {
                    bool win = true;
                    for (int k = 0; k < winSize; k++)
                    {
                        if (cells[(row + k) * n + col] != player)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        return true;
                    }
                }
            }

            // Диагонали (слева-направо, сверху-вниз):
            for (int row = 0; row <= n - winSize; row++)
            {
                for (int col = 0; col <= n - winSize; col++)
                {
                    bool win = true;
                    for (int k = 0; k < winSize; k++)
                    {
                        if (cells[(row + k) * n + col + k] != player)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        return true;
                    }
                }
            }

            // Другие диагонали (справа-налево, сверху-вниз):
            for (int row = 0; row <= n - winSize; row++)
            {
                for (int col = n - 1; col >= winSize - 1; col--)
                {
                    bool win = true;
                    for (int k = 0; k < winSize; k++)
                    {
                        if (cells[(row + k) * n + col - k] != player)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsFirstMove(CellEnum[] cells)
        {
            // Проверка, что все ячейки пустые
            return cells.All(cell => cell == CellEnum.None);
        }

        private MoveGameDto MakeFirstMove(Game game, int userId, CellEnum cellBot, CellEnum[] cells)
        {
            int n = (int)Math.Sqrt(cells.Length);
            int move;

            // Если размер поля нечётный, выбираем центр
            if (n % 2 == 1)
            {
                move = (n / 2) * n + (n / 2);
            }
            else
            {
                // Иначе выбираем случайную клетку
                var random = new Random();
                do
                {
                    move = random.Next(0, cells.Length);
                } while (cells[move] != CellEnum.None);
            }

            return new MoveGameDto
            {
                GameId = game.Id,
                UserId = userId,
                NumberCell = move,
                Cell = cellBot,
                IsMoveWin = false,
                isDrawGame = false
            };
        }
    }
}
