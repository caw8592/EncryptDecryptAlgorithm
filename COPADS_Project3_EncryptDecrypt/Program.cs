#pragma warning disable

using SkiaSharp;

namespace Project {
    class Project {
        static void Main(string[] args) {
            if(args.Length == 0) {
                Console.WriteLine("Usage: dotnet run <option> <...>\n" + 
                    "   options - Cipher, GenerateKeystream, Encrypt, Decrypt, " + 
                    "TripleBit, EncryptImage, DecryptImage");
                return;
            }
            int tap; int step;
            switch(args[0].ToLower()) {
                case "cipher": 
                    if(args.Length == 3 && int.TryParse(args[2], out tap)) {
                        Console.WriteLine($"{args[1]} -seed");
                        cipher(getBits(args[1]), tap);
                    } else 
                        Console.WriteLine("Usage: dotnet run Cipher <seed> <tap>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position");
                break;
                case "generatekeystream": 
                    if(args.Length == 4 && int.TryParse(args[2], out tap) && int.TryParse(args[3], out step) && step > 0) {
                        Console.WriteLine($"{args[1]} -seed");
                        generatekeystream(getBits(args[1]), tap, step);
                    } else 
                        Console.WriteLine("Usage: dotnet run GenerateKeystream <seed> <tap> <step>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position\n" +
                            "   step - The number of steps");
                break;
                case "encrypt": 
                    if(args.Length == 2) {
                        var result = encrypt_decrypt(getBits(args[1]));
                        if(result != null) Console.WriteLine($"The ciphertext is: {bits_to_string(result)}");
                    } else 
                        Console.WriteLine("Usage: dotnet run Encrypt <plaintext>\n" +
                            "   plaintext - The plaintext in bits");
                break;
                case "decrypt":
                    if(args.Length == 2) {
                        var result = encrypt_decrypt(getBits(args[1]));
                        if(result != null) Console.WriteLine($"The plaintext is: {bits_to_string(result)}");
                    } else 
                        Console.WriteLine("Usage: dotnet run Decrypt <plaintext>\n" +
                            "   plaintext - The plaintext in bits");
                break;
                case "triplebits": 
                    if(args.Length == 5 && int.TryParse(args[2], out tap) && int.TryParse(args[3], out step) 
                        && step > 0 && int.TryParse(args[4], out int iteration) && iteration > 0) {
                            Console.WriteLine($"{args[1]} -seed");
                            triplebits(getBits(args[1]), tap, step, iteration);
                    } else 
                        Console.WriteLine("Usage: dotnet run TripleBit <seed> <tap> <step> <iteration>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position\n" +
                            "   step - The number of steps\n" +
                            "   iteration - The number of iterations");
                break;
                case "encryptimage": 
                    if(args.Length == 4 && int.TryParse(args[3], out tap)) {
                        encrypt_decrypt_image(args[1], args[2], tap);
                    } else 
                        Console.WriteLine("Usage: dotnet run EncryptImage <imagefile> <seed> <tap>\n" +
                            "   imagefile - The file path for the image\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position");
                break;
                case "decryptimage": 
                    if(args.Length == 4 && int.TryParse(args[3], out tap)) {
                        encrypt_decrypt_image(args[1], args[2], tap, true);
                    } else 
                        Console.WriteLine("Usage: dotnet run DecryptImage <imagefile> <seed> <tap>\n" +
                            "   imagefile - The file path for the image\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position");
                break;
                default: 
                    Console.WriteLine("Usage: dotnet run <option> <...>\n" + 
                    "   - options: Cipher, GenerateKeystream, Encrypt, Decrypt, " + 
                    "TripleBit, EncryptImage, DecryptImage");
                break;
            }
        }

        static int[] getBits(string str) {
            var bits = new int[str.Length];
            for(int i = 0; i<str.Length; i++) {
                bits[i] = str[i] == '1' ? 1 : 0;
            }
            return bits;
        }

        static string bits_to_string(int[] bits){
            var result = "";
            foreach(int bit in bits) {
                result += $"{bit}";
            }
            return result;
        }
 
        static (int[] bits, int right_most) cipher(int[] bits, int tap, bool no_out = false) {

            var new_bit = bits[tap] ^ bits[0];

            for(int i = 0; i<bits.Length-1; i++) {
                bits[i] = bits[i+1];
            }

            bits[bits.Length-1] = new_bit;

            if(!no_out) Console.WriteLine($"{bits_to_string(bits)} {new_bit}");
            return (bits, new_bit);
        }

        static void generatekeystream(int[] bits, int tap, int step) {
            string keystream = "";
            using(StreamWriter file = new StreamWriter("keystream.txt")) {
                for(int i = 0; i<step; i++) {
                    var result = cipher(bits, tap);
                    bits = result.bits;
                    file.Write($"{result.right_most}");
                    keystream += result.right_most;
                }
            }
            Console.WriteLine($"The Keystream: {keystream}");
        }

        static int[] encrypt_decrypt(int[] text) {
            int[] keystream;
            try{
                using(StreamReader file = new StreamReader("keystream.txt")) {
                    var keystream_string = file.ReadLine()+"";
                    keystream = getBits(keystream_string);
                }
            } catch {
                Console.WriteLine("Please Generate a Keystream.");
                return null;
            }

            int encryption_size = (keystream.Length > text.Length) ? keystream.Length : text.Length;
            int[] encrypted_text = new int[encryption_size];

            if(keystream.Length != text.Length) {
                int[] zeros = new int[encryption_size];
                var pos = Math.Abs(keystream.Length - text.Length);
                if(keystream.Length > text.Length) {
                    Array.Copy(text, 0, zeros, pos, text.Length);
                    text = zeros;
                }else if(keystream.Length < text.Length) {
                    Array.Copy(keystream, 0, zeros, pos, keystream.Length);
                    keystream = zeros;
                }
            }

            for(int i = 0; i<encryption_size; i++) {
                encrypted_text[i] = text[i] ^ keystream[i];
            }

            return encrypted_text;
        }

        static void triplebits(int[] bits, int tap, int step, int iteration) {
            for(int iter = 0; iter < iteration; iter++) {
                var accumulated_int = 1;

                for(int stp = 0; stp < step; stp++) {
                    var result = cipher(bits, tap, true);
                    bits = result.bits;

                    accumulated_int *= 3;
                    accumulated_int += result.right_most;
                }

                Console.WriteLine($"{bits_to_string(bits)} {accumulated_int}");
            }
        }

        static void encrypt_decrypt_image(string image_file, string seed, int tap, bool decrypt = false) {
            var cipher_call = cipher(getBits(seed), tap, true);
            string new_seed = bits_to_string(cipher_call.bits);

            using(SKBitmap image = SKBitmap.Decode(image_file)) {
                if(image == null) {
                    Console.WriteLine("The given image file does not exist");
                    return;
                }
                
                var ran = new Random(Convert.ToInt32(new_seed, 2));

                for(int height = 0; height<image.Height; height++) {
                    for(int width = 0; width<image.Width; width++) {
                        var pixel_color = image.GetPixel(width, height);
                        
                        var red = (byte) (pixel_color.Red ^ (byte) ran.Next(0, 256));
                        var green = (byte) (pixel_color.Green ^ (byte) ran.Next(0, 256));
                        var blue = (byte) (pixel_color.Blue ^ (byte) ran.Next(0, 256));

                        image.SetPixel(width, height, new SKColor(red, green, blue));
                    }
                }
                
                var len = image_file.Contains("ENCRYPTED") ? image_file.Length-13 : image_file.Length-4;
                var file_name = image_file.Substring(0, len);
                var save_loc = decrypt ? $"{file_name}New.png" : $"{file_name}ENCRYPTED.png";
                using(var file = new FileStream(save_loc, FileMode.Create)) {
                    SKImage.FromBitmap(image).Encode(SKEncodedImageFormat.Png, 100).SaveTo(file);
                }
            }
        }
    }
}