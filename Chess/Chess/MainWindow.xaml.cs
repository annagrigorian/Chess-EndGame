﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rd = new Random();
        static Image[,] board = new Image[8, 8];
        static bool[,] isTaken = new bool[8, 8];
        Image whiteking;
        Image blackking;
        Image blackrook;
        Image figuretomove;
        double top, left;
        static bool isMoving;
        static bool isWhiteTurn = true;
        int stepsCount = 0;
        static double vectorleft, vectortop;
        static Thickness startpos;
        List<Thickness> whitekingfields;


        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new Image();
                    board[i, j].Source = new BitmapImage(new Uri(((i + j) % 2 == 1) ? "whitem.jpg" : "gray.jpg", UriKind.Relative));
                    board[i, j].Width = 50;
                    board[i, j].Height = 50;
                    board[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                    board[i, j].VerticalAlignment = VerticalAlignment.Top;
                    board[i, j].Margin = new Thickness(i * 50, j * 50, 0, 0);
                    grid.Children.Add(board[i, j]);
                }
            }

            int blackkingX = rd.Next(0, 8);
            int blackkingY = rd.Next(0, 8);
            blackking = CreateAndPlaceFigure("bk", blackkingX, blackkingY);

        tryagain1:
            int blackrookX = rd.Next(0, 8);
            int blackrookY = rd.Next(0, 8);
            if (blackkingX == blackrookX && blackkingY == blackrookY)
            {
                goto tryagain1;
            }
            else
            {
                blackrook = CreateAndPlaceFigure("br", blackrookX, blackrookY);
            }


        tryagain2:
            int whitekingX = rd.Next(0, 8);
            int whitekingY = rd.Next(0, 8);

            if ((whitekingX == blackrookX && whitekingY == blackrookY)
                || (whitekingX == blackkingX && whitekingY == blackkingY)
                || (whitekingX == blackrookX || whitekingY == blackrookY)
                || (Math.Abs(whitekingX - blackkingX) <= 1 || Math.Abs(whitekingY - blackkingY) <= 1)
                || (Math.Abs(whitekingX - blackrookX) <= 1 || Math.Abs(whitekingY - blackrookY) <= 1))
            {
                goto tryagain2;
            }
            else
            {
                whiteking = CreateAndPlaceFigure("wk", whitekingX, whitekingY);
            }

            whiteking.MouseDown += Whiteking_MouseDown;
            grid.MouseUp += Grid_MouseUp;
            grid.MouseMove += Grid_MouseMove;

        }


        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMoving)
                return;

            figuretomove.Margin = new Thickness(e.GetPosition(this).X - vectorleft, e.GetPosition(this).Y - vectortop, 0, 0);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            bool allowtoput = false;
            isMoving = false;

            if (whitekingfields != null)
            {

                for (int i = 0; i < whitekingfields.Count; i++)
                {
                    if (Math.Abs(whiteking.Margin.Left - whitekingfields[i].Left) < 30 &&
                        Math.Abs(whiteking.Margin.Top - whitekingfields[i].Top) < 30)
                    {
                        isTaken[(int)whiteking.Margin.Top / 50, (int)whiteking.Margin.Left / 50] = false;
                        whiteking.Margin = whitekingfields[i];
                        isTaken[(int)whiteking.Margin.Top / 50, (int)whiteking.Margin.Left / 50] = true;
                        allowtoput = true;
                        isWhiteTurn = false;
                        break;
                    }
                }
                if (!allowtoput)
                    whiteking.Margin = startpos;

                if (!isWhiteTurn)
                {
                    List<Thickness> rand = GetRookFields(blackrook.Margin);

                    blackrook.Margin = rand[rd.Next(0, rand.Count)];

                    isWhiteTurn = true;
                }
            }
        }

        private void Whiteking_MouseDown(object sender, MouseButtonEventArgs e)
        {

            isMoving = true;
            figuretomove = whiteking;
            vectorleft = (int)e.GetPosition(this).X - figuretomove.Margin.Left;
            vectortop = (int)e.GetPosition(this).Y - figuretomove.Margin.Top;
            startpos = figuretomove.Margin;

            whitekingfields = GetKingFields(whiteking, whiteking.Margin);
        }

        public Image CreateAndPlaceFigure(string figurename, int x, int y)
        {
            Image newfigure = new Image();

            switch (figurename)
            {
                case "bk":
                    newfigure.Source = new BitmapImage(new Uri("bk.gif", UriKind.Relative));
                    break;
                case "br":
                    newfigure.Source = new BitmapImage(new Uri("br.gif", UriKind.Relative));
                    break;
                case "wk":
                    newfigure.Source = new BitmapImage(new Uri("wk.gif", UriKind.Relative));
                    break;
            }

            newfigure.Width = 50;
            newfigure.Height = 50;
            newfigure.HorizontalAlignment = HorizontalAlignment.Left;
            newfigure.VerticalAlignment = VerticalAlignment.Top;
            newfigure.Margin = new Thickness(x * 50, y * 50, 0, 0);
            grid.Children.Add(newfigure);

            isTaken[(int)newfigure.Margin.Top / 50, (int)newfigure.Margin.Left / 50] = true;

            return newfigure;
        }

        public static bool InBound(int cx, int cy)
        {
            return (cx >= 0 && cx < 8 && cy < 8 && cy >= 0);
        }

        public void BlacksTurn()
        {
            int WhiteKingX = (int)(whiteking.Margin.Left * 50) / 50;
            int WhiteKingY = (int)(whiteking.Margin.Top * 50) / 50;

            List<Thickness> thicks = GetRookFields(blackrook.Margin);
            List<Thickness> possibleThicks = new List<Thickness>();

            List<int> areas = new List<int>();
            areas.Add(WhiteKingX / 50);////left-0
            areas.Add(WhiteKingY / 50);////up-1
            areas.Add((400 - (WhiteKingX + 50)) / 50);///right-2
            areas.Add((400 - (WhiteKingY + 50)) / 50);////down-3

            int minArea = areas[0];
            int minInd = 0;
            foreach (var item in areas)
            {
                if (item < minArea)
                {
                    minArea = item;
                    minInd = areas.IndexOf(item);
                }
            }
            switch (minInd)
            {
                case 0:
                    left = WhiteKingX + 50;
                    top = WhiteKingY;
                    foreach (var item in thicks)
                    {
                        if (item.Left == left && item.Top != top && item.Top != top - 50 && item.Top != top + 50)
                        {
                            possibleThicks.Add(item);
                        }
                    }
                    break;
                case 1:
                    top = WhiteKingY + 50;
                    left = WhiteKingX;
                    foreach (var item in thicks)
                    {
                        if (item.Top == top && item.Left != left && item.Left != left - 50 && item.Left != left + 50)
                        {
                            possibleThicks.Add(item);
                        }
                    }
                    break;
                case 2:
                    left = WhiteKingX - 50;
                    top = WhiteKingY;
                    foreach (var item in thicks)
                    {
                        if (item.Left == left && item.Top != top && item.Top != top - 50 && item.Top != top + 50)
                        {
                            possibleThicks.Add(item);
                        }
                    }
                    break;
                case 3:
                    top = WhiteKingY - 50;
                    left = WhiteKingX;
                    foreach (var item in thicks)
                    {
                        if (item.Top == top && item.Left != left && item.Left != left - 50 && item.Left != left + 50)
                        {
                            possibleThicks.Add(item);
                        }
                    }
                    break;
            }
            if (possibleThicks.Contains(blackking.Margin))
                possibleThicks.Remove(blackking.Margin);
            if (possibleThicks.Count == 0)
            {
                if (stepsCount == 1)
                {
                    if (blackking.Margin.Left > blackrook.Margin.Left)
                    {
                        foreach (var item in thicks)
                        {
                            if (item.Left == 0)
                            {
                                blackrook.Margin = item;
                                return;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in thicks)
                        {
                            if (item.Left == 350)
                            {
                                blackrook.Margin = item;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    ///gnal harmar dirq
                }
            }
            if (possibleThicks.Count == 1)
            {
                blackrook.Margin = possibleThicks[0];
                return;
            }
            if (possibleThicks.Count > 1)
            {
                ///=> arqayov qayl
                /// 

                return;
            }
            #region old
            //int fromTop = (int)whiteking.Margin.Top / 50;
            //int fromLeft = (int)whiteking.Margin.Left / 50;
            //int fromButtom = 8 - fromTop;
            //int fromRight = 8 - fromLeft;

            //int minDistanceHorizontal = Math.Min(fromLeft, fromRight);
            //int minDistanceVertical = Math.Min(fromTop, fromButtom);

            //if (minDistanceHorizontal <= minDistanceVertical)
            //{
            //    if ()
            //    {

            //    }
            //}
            #endregion
        }

        public static List<Thickness> GetKingFields(Image king, Thickness kingpos)
        {
            List<Thickness> kingfields = new List<Thickness>();

            int currentX, currentY;
            currentX = (int)(kingpos.Left / 50);
            currentY = (int)(kingpos.Top / 50);

            if (InBound(currentX, currentY + 1))
                kingfields.Add(board[currentX, currentY + 1].Margin);
            if (InBound(currentX, currentY - 1))
                kingfields.Add(board[currentX, currentY - 1].Margin);

            if (InBound(currentX - 1, currentY - 1))
                kingfields.Add(board[currentX - 1, currentY - 1].Margin);
            if (InBound(currentX - 1, currentY))
                kingfields.Add(board[currentX - 1, currentY].Margin);
            if (InBound(currentX - 1, currentY + 1))
                kingfields.Add(board[currentX - 1, currentY + 1].Margin);

            if (InBound(currentX + 1, currentY + 1))
                kingfields.Add(board[currentX + 1, currentY + 1].Margin);
            if (InBound(currentX + 1, currentY - 1))
                kingfields.Add(board[currentX + 1, currentY - 1].Margin);
            if (InBound(currentX + 1, currentY))
                kingfields.Add(board[currentX + 1, currentY].Margin);

            return kingfields;
        }

        public static List<Thickness> GetRookFields(Thickness rookpos)
        {
            List<Thickness> rookfields = new List<Thickness>();

            int cx, cy;
            cx = (int)rookpos.Left / 50;
            cy = (int)rookpos.Top / 50;

            while (InBound(cx, --cy) && !(isTaken[cy, cx]))
            {
                rookfields.Add(board[cx, cy].Margin);
            }

            cx = (int)rookpos.Left / 50;
            cy = (int)rookpos.Top / 50;
            while (InBound(cx, ++cy) && !(isTaken[cy, cx]))
            {
                rookfields.Add(board[cx, cy].Margin);
            }

            cx = (int)rookpos.Left / 50;
            cy = (int)rookpos.Top / 50;
            while (InBound(--cx, cy) && !(isTaken[cy, cx]))
            {
                rookfields.Add(board[cx, cy].Margin);
            }

            cx = (int)rookpos.Left / 50;
            cy = (int)rookpos.Top / 50;
            while (InBound(++cx, cy) && !(isTaken[cy, cx]))
            {
                rookfields.Add(board[cx, cy].Margin);
            }


            return rookfields;


        }
    }

}
