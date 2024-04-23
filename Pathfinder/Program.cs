using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    /* zdroje:
    tady jsou vsechny zdroje nejake jsem pro kod vubec nepouzil ale pro pochopeni pathfind algoritmu atd.  Pak jsou jeste u casti v kodu presne zdroje
    https://www.youtube.com/watch?v=-L-WgKMFuhE
    https://en.wikipedia.org/wiki/Pathfinding
    https://en.wikipedia.org/wiki/Breadth-first_search
    https://www.youtube.com/watch?v=HZ5YTanv5QE
    https://www.youtube.com/watch?v=XU_ugVTSjSE&t=5s
    https://www.geeksforgeeks.org/breadth-first-traversal-bfs-on-a-2d-array/
    https://www.w3schools.com/python/ref_list_sort.asp
    https://learn.microsoft.com/cs-cz/dotnet/api/system.collections.queue?view=net-7.0
    https://www.youtube.com/watch?v=-L-WgKMFuhE&t=497s
    chat gpt bylo pouzito napric kodem pro jeho přehleduplnost a lehčí úpravy
    poznámka: nejake casti kodu se opakuji kvuli podobnemu algoritmu. Odkaz na inspiraci bude teda u jedne casti kodu to nemusim delat vsude.
    */

    // proměná pro zadání
    static char[,] mapa = new char[,]
    {
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'.', '.', '.', '.', '.', '.', 'H', '.', '.', '.', '.', '.', '.'},
        {'#', '.', '.', 'K', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', 'K', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'},
        {'#', '.', '.', '.', '#', '.', '.', '.', '#', '.', '.', '.', '.'}
    };

    static void Main(string[] args)
    {
        // souradnice -1 kdyby se nenasla hracka ci kocka
        int kockaX = -1;
        int kockaY = -1;
        int hrackaX = -1;
        int hrackaY = -1;
        List<(int, int)> hracky = new List<(int, int)>();
        bool cmd = true;
        int kockaCount = 0;

        for (int i = 0; i < mapa.GetLength(0); i++)
        {
            for (int j = 0; j < mapa.GetLength(1); j++)
            {
                char ted = mapa[i, j];

                if (ted == 'K')
                {   // kocka je vzdy jenom jedna
                    kockaX = i;
                    kockaY = j;
                    kockaCount++;
                }
                else if (ted == 'H') 
                {   //promene pro 1/2 mód 
                    hrackaX = i; 
                    hrackaY = j; 

                    int hrackyX = i;
                    int hrackyY = j;
                    // 1 a vice hracek pro 3 mód
                    hracky.Add((hrackyX, hrackyY)); 
                }
            }
        }
        // pokud vic jak jedna kocka ukoncit proces
        if (kockaCount>1) 
        {
            Console.WriteLine("vic jak jedna kocka");
            return;
        }
        //basic user interface pro lepsi testovani a outupu
        while (cmd)
        {
            Console.WriteLine("zvol mód. Stiskni 1/2/3.");

            ConsoleKeyInfo mod = Console.ReadKey(true);

            switch (mod.Key)
            {   // mod 1
                case ConsoleKey.D1 :
                case ConsoleKey.NumPad1 :
                    bool odpoved = JeCestaNeboNe(kockaX, kockaY, hrackaX, hrackaY);
                    if (odpoved)
                    {
                        Console.WriteLine("ANO");
                    }
                    else
                    {
                        Console.WriteLine("NE");
                    }
                    break;
                // mod 2
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2 :
                    List<(int, int)> cesta = MapaCesty(kockaX, kockaY, hrackaX, hrackaY);


                    if (cesta != null)
                    {
                        //posledni koordinace
                        int posledni = cesta.Count - 1; 
                        for (int i = 0; i < cesta.Count; i++)
                        {
                            
                            var bod = cesta[i];
                            //vypsani souradnice
                            Console.Write($"{bod.Item1}x{bod.Item2}");

                           
                            if (i != posledni)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("NE");
                    }
                    break;
                //mod 3
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    List<List<(int, int)>> cesty = ViceCest(kockaX, kockaY, hracky);
                   
                    if (cesty != null)
                    {
                        // sort pomoc https://www.w3schools.com/python/ref_list_sort.asp
                        cesty.Sort((path1, path2) => path1.Count.CompareTo(path2.Count));
                    }


                    if (cesty != null)
                    {
                        // ulozeni indexu posledni cesty
                        int poslednicesta = cesty.Count - 1; 
                        for (int j = 0; j < cesty.Count; j++)
                        {
                            var path = cesty[j];
                            //ulozeni indexu posledni souradnice
                            int posledni = path.Count - 1; 
                            for (int i = 0; i < path.Count; i++)
                            {
                                var bod = path[i];
                                Console.Write($"{bod.Item1}x{bod.Item2}");

                                
                                if (i != posledni)
                                {
                                    Console.Write(", ");
                                }
                            }

                            
                            if (j != poslednicesta)
                            {
                                Console.Write(" | ");
                            }
                        }
                        Console.Write('.');
                        Console.WriteLine(); 
                    }
                    else
                    {
                        Console.WriteLine("NE");
                    }
                    break;
            }
        }
    }
    //mod 2

    /* 
     
    nejvetsi inspirace algoritmů z https://www.geeksforgeeks.org/breadth-first-traversal-bfs-on-a-2d-array/
                                   https://www.youtube.com/watch?v=XU_ugVTSjSE&t=5s
        
    */
    static List<(int, int)> MapaCesty(int startX, int startY, int hrackaX, int hrackaY)
    {
        // queue ma v sobe prave overovene policko
        // queue pomoc https://learn.microsoft.com/cs-cz/dotnet/api/system.collections.queue?view=net-7.0
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));
        // zachovava zaznam cesty
        Dictionary<(int, int), (int, int)> zaznamCesty = new Dictionary<(int, int), (int, int)>();
        zaznamCesty.Add((startX, startY), (-1, -1));
        //dokud se nenajde cil nebo neprojdou vsechny moznosti
        while (queue.Count > 0)
        {
            var ted_policko = queue.Dequeue();
            int x = ted_policko.Item1;
            int y = ted_policko.Item2;
            //pokud se najde cil return cesta
            if (x == hrackaX && y == hrackaY)
            {
                // rekonstuovat cestu
                List<(int, int)> path = new List<(int, int)>();
                while (x != -1 && y != -1)
                {
                    path.Add((x, y));
                    (x, y) = zaznamCesty[(x, y)];
                }
                path.Reverse();
                return path;
            }

            // podivat se na sousedy
            int[] soused_souradnice_x = { -1, 1, 0, 0 };
            int[] soused_souradnice_y = { 0, 0, -1, 1 };
            // dat do queue sousedy provereneho policka a overit zed
            for (int i = 0; i < 4; i++)
            {
                int newX = x + soused_souradnice_x[i];
                int newY = y + soused_souradnice_y[i];

                if (newX >= 0 && newX < mapa.GetLength(0) &&
                    newY >= 0 && newY < mapa.GetLength(1) &&
                    mapa[newX, newY] != '#' &&
                    !zaznamCesty.ContainsKey((newX, newY)))
                {
                    queue.Enqueue((newX, newY));
                    zaznamCesty[(newX, newY)] = (x, y);
                }
            }
        }


        return null;
    }

    //mod 1 
    static bool JeCestaNeboNe(int startX, int startY, int hrackaX, int hrackaY)
    {   // queue ma v sobe prave overovene policko
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));

        List<(int, int)> navstiveno = new List<(int, int)>();
        navstiveno.Add((startX, startY));
        //dokud se nenajde cil nebo neprojdou vsechny moznosti
        while (queue.Count > 0)
        {
            var ted_policko = queue.Dequeue();
            int x = ted_policko.Item1;
            int y = ted_policko.Item2;
            //pokud se najde cil return true
            if (x == hrackaX && y == hrackaY)
            {
                return true;
            }

            // podivat se na sousedy
            int[] soused_souradnice_x = { -1, 1, 0, 0 };
            int[] soused_souradnice_y = { 0, 0, -1, 1 };
            // dat do queue sousedy provereneho policka a overit zed
            for (int i = 0; i < 4; i++)
            {
                int newX = x + soused_souradnice_x[i];
                int newY = y + soused_souradnice_y[i];

                if (newX >= 0 && newX < mapa.GetLength(0) &&
                    newY >= 0 && newY < mapa.GetLength(1) &&
                    mapa[newX, newY] != '#' &&
                    !navstiveno.Contains((newX, newY)))
                {
                    queue.Enqueue((newX, newY));
                    navstiveno.Add((newX, newY));
                }
            }
        }

        
        return false;
    }




    //mod 3
    static List<List<(int, int)>> ViceCest(int startX, int startY, List<(int, int)> hracky)
    {   // list vsech najitich cest
        List<List<(int, int)>> paths = new List<List<(int, int)>>();
        //projde vsechny cile
        foreach (var hracka in hracky)
        {
            int hrackaX = hracka.Item1;
            int hrackaY = hracka.Item2;
            //overovane policka
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));
            //zaznam cesty pro hledane policko
            Dictionary<(int, int), (int, int)> zaznamCesty= new Dictionary<(int, int), (int, int)>();
            zaznamCesty.Add((startX, startY), (-1, -1));
            //dokud nenajde cestu nebo vsechna policka projde
            while (queue.Count > 0)
            {
                var ted_policko = queue.Dequeue();
                int x = ted_policko.Item1;
                int y = ted_policko.Item2;
                // pokud najde cestu prida ji do listu 
                if (x == hrackaX && y == hrackaY)
                {
                    
                    List<(int, int)> path = new List<(int, int)>();
                    while (x != -1 || y != -1)
                    {
                        path.Add((x, y));
                        (x, y) = zaznamCesty[(x, y)];
                    }
                    path.Reverse();
                    paths.Add(path);
                    break; 
                }

                //prozkoumat sousedy
                int[] soused_souradnice_x = { -1, 1, 0, 0 };
                int[] soused_souradnice_y = { 0, 0, -1, 1 };
                //prida sousedy do queue a zjistit pokud to je zed
                for (int i = 0; i < 4; i++)
                {
                    int newX = x + soused_souradnice_x[i];
                    int newY = y + soused_souradnice_y[i];

                    if (newX >= 0 && newX < mapa.GetLength(0) &&
                        newY >= 0 && newY < mapa.GetLength(1) &&
                        mapa[newX, newY] != '#' &&
                        !zaznamCesty.ContainsKey((newX, newY)))
                    {
                        queue.Enqueue((newX, newY));
                        zaznamCesty[(newX, newY)] = (x, y);
                    }
                }
            }
        }
        if (paths.Count == 0)
        {
            return null;
        }

        return paths;
    }
}