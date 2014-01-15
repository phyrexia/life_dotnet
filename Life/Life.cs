using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life
{
    class Life
    {
        private const int DEFAULT_BOARD_WIDTH = 50;
        private const int DEFAULT_BOARD_HEIGHT = 20;
        private const bool ALIVE = true;
        private const bool DEAD = false;

        public int width { get; set; }

        public int height { get; set; }

        public bool toroidal { get; set; }

        private Stack<bool[,]> lifeHistory;

        public Life()
            : this(DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT, false)
        { }

        public Life(int width, int height)
            : this(width, height, false)
        { }

        public Life(bool toroidal)
            : this(DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT, toroidal)
        { }

        public Life(int width, int height, bool toroidal)
        {
            this.width = width;
            this.height = height;
            this.toroidal = toroidal;

            bool[,] lifeBoard = new bool[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    lifeBoard[y, x] = DEAD;
                }
            }

            lifeHistory = new Stack<bool[,]>();
            lifeHistory.Push(lifeBoard);
        }

        public bool CycleCell(int xCoord, int yCoord)
        {
            return lifeHistory.Peek()[yCoord, xCoord] = !lifeHistory.Peek()[yCoord, xCoord];
        }

        public bool GetCell(int xCoord, int yCoord)
        {
            if (IsCellCoordinateValid(xCoord, yCoord))
            {
                return lifeHistory.Peek()[yCoord, xCoord];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid cell coordinate, (" + xCoord + ", " + yCoord + ")!");
            }
        }

        public bool IsCellCoordinateValid(int xCoord, int yCoord)
        {
            return xCoord >= 0 && yCoord >= 0 && xCoord < width && yCoord < height;
        }

        public void step()
        {
            bool[,] newBoard = new bool[height, width];
            int numberNeighbors;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    numberNeighbors = GetNumberNeighbors(x, y);
                    if (lifeHistory.Peek()[y, x])
                    { //alive
                        if (numberNeighbors == 3 || numberNeighbors == 2)
                        {
                            newBoard[y, x] = ALIVE;
                        }
                    }
                    else
                    { //dead
                        if (numberNeighbors == 3)
                        {
                            newBoard[y, x] = ALIVE; //reproduction
                        }
                    }
                }
            }
            lifeHistory.Push(newBoard);
        }

        public int GetNumberNeighbors(int xCoord, int yCoord)
        {
            int numNeighbors = 0;
            if (toroidal)
            {
                for (int neighborYpos = yCoord - 1; neighborYpos <= yCoord + 1; neighborYpos++)
                {
                    for (int neighborXpos = xCoord - 1; neighborXpos <= xCoord + 1; neighborXpos++)
                    {
                        int toroidalYpos, toroidalXpos;

                        if (neighborYpos < 0)
                        {
                            toroidalYpos = height + neighborYpos;
                        }
                        else if (neighborYpos >= height)
                        {
                            toroidalYpos = neighborYpos - height;
                        }
                        else
                        {
                            toroidalYpos = neighborYpos;
                        }

                        if (neighborXpos < 0)
                        {
                            toroidalXpos = width + neighborXpos;
                        }
                        else if (neighborXpos >= width)
                        {
                            toroidalXpos = neighborXpos - width;
                        }
                        else
                        {
                            toroidalXpos = neighborXpos;
                        }

                        if (IsCellCoordinateValid(toroidalXpos, toroidalYpos))
                        {
                            if ((toroidalXpos != xCoord || toroidalYpos != yCoord) && lifeHistory.Peek()[toroidalYpos, toroidalXpos])
                                numNeighbors++;
                        }
                    }
                }
            }
            else
            {
                for (int neighborYpos = yCoord - 1; neighborYpos <= yCoord + 1; neighborYpos++)
                {
                    for (int neighborXpos = xCoord - 1; neighborXpos <= xCoord + 1; neighborXpos++)
                    {
                        if (IsCellCoordinateValid(neighborXpos, neighborYpos))
                        {
                            if ((neighborXpos != xCoord || neighborYpos != yCoord) && lifeHistory.Peek()[neighborYpos, neighborXpos])
                                numNeighbors++;
                        }
                    }
                }
            }
            return numNeighbors;
        }

        public void Extinction()
        {
            lifeHistory.Clear();
            lifeHistory.Push(new bool[height, width]);
        }

        public void Randomize(bool clearStack)
        {
            if (clearStack)
            {
                Extinction();
            }
            Random random = new Random();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (random.Next(2) == 1)
                    {
                        CycleCell(x, y);
                    }
                }
            }
        }

        public void Invert()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CycleCell(x, y);
                }
            }
        }

        public int GetGeneration()
        {
            return lifeHistory.Count;
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder("Generation ");
            output.Append(lifeHistory.Count);
            output.Append("\n-1");
            // first line
            for (int q = 0; q < width - 1; q++)
            {
                output.Append("-");
            }
            output.Append("\n");

            for (int h = 0; h < height; h++)
            { // go down the board
                for (int w = 0; w < width; w++)
                { // go across the board
                    if (w == 0)
                    {
                        if (h == 0) // first line only
                            output.Append("1");
                        else
                            output.Append("|");
                    }
                    if (lifeHistory.Peek()[h, w])
                    {
                        output.Append("@");
                    }
                    else
                    {
                        output.Append(" ");
                    }
                    if (w == width - 1)
                        output.Append("|");
                }
                output.Append("\n");
            }
            for (int q = 0; q <= width + 1; q++)
            {
                output.Append("-");
            }
            output.Append("\n");
            return output.ToString();
        }

        static void Main(string[] args)
        {
            Life life = new Life(true);
            life.Randomize(true);
            while (life.GetGeneration() < 1000)
            {
                Console.WriteLine(life);
                try
                {
                    System.Threading.Thread.Sleep(250);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
                life.step();
            }
        }
    }
}
