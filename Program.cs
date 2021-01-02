using System;
using System.IO;
using System.Threading;

namespace hash_cracker
{
    class Program
    {
        public static string MD5Hash(String input){
            using(System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()){
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for(int i = 0; i < hashBytes.Length; i++){
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        static void Main(string[] args)
        {
            void message(String text, bool nl = true, ConsoleColor col = ConsoleColor.White){
                Console.ForegroundColor = col;
                Console.Write(">>> ");
                Console.ForegroundColor = ConsoleColor.White;
                if (nl) { Console.WriteLine(text); } else { Console.Write(text); }
            }

            void throw_error(String text){
                message(text, col: ConsoleColor.Red);
                Console.ReadLine();
                Environment.Exit(0);
            }

            message("Hashed password: ", nl: false, col: ConsoleColor.Blue);
            String hash = Console.ReadLine();
            message("Dictionary file: ", nl: false, col: ConsoleColor.Blue);
            String filename = Console.ReadLine();

            if(hash == "" || filename == "") throw_error("Please specify hash and dictionary!");
            if(hash.Length != 32) throw_error("This is not MD5 hash!");
            if(!File.Exists("./" + filename)) throw_error("Specified dictionary does not exist!");
            String[] passes = File.ReadAllLines(filename);

            bool verbose;
            message("Show percentage? [y/n]: ", nl: false, col: ConsoleColor.Blue);
            String answ = Console.ReadLine();
            switch(answ.ToLower()){
                case "y":
                    verbose = true;
                break;
                case "n":
                    verbose = false;
                break;
                default:
                    message("Wrong answer, continuing without percentage!", col: ConsoleColor.Red);
                    verbose = false;
                break;
            }

            // 7b24afc8bc80e548d66c4e7ff72171c5 => toor
            // 0d3d238b089a67e34e39b5abf80db19b => rot (this will not work with standart dictionary)

            message("Start", col: ConsoleColor.Yellow);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for(int j = 0; j < passes.Length; j++){
                if(verbose && j % 1000 == 0) Console.Write($"\r{Math.Round((j+1.0f)/passes.Length*100.0f, 4)}%: {passes[j]}     ");
                if(MD5Hash(passes[j]).ToLower() == hash){
                    if(verbose) Console.WriteLine("\r                                     ");
                    message($"Match Found: {passes[j]}", col: ConsoleColor.Green);
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    message($"End in {elapsedMs} milliseconds ({Convert.ToSingle(elapsedMs)/1000.0f} sec)", col: ConsoleColor.Yellow);
                    Console.ReadLine();
                    return;
                }
            }
            message("Password not cracked!", col: ConsoleColor.Red);
            Console.ReadLine();
        }
    }
}
