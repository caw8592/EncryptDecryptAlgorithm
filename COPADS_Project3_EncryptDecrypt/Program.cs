

namespace Project {
    class Project {
        static void Main(string[] args) {
            int tap; int step;
            switch(args[0].ToLower()) {
                case "cipher": 
                    if(args.Length == 3 && int.TryParse(args[2], out tap)) {
                        Console.WriteLine($"{args[1]} -seed");
                        cipher(getBits(args[1]), tap);
                    } else 
                        Console.WriteLine("Usage: Cipher <seed> <tap>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position");
                break;
                case "generatekeystream": 
                    if(args.Length == 4 && int.TryParse(args[2], out tap) && int.TryParse(args[3], out step) && step > 0) {
                        Console.WriteLine($"{args[1]} -seed");
                        generatekeystream(getBits(args[1]), tap, step);
                    } else 
                        Console.WriteLine("Usage: GenerateKeystream <seed> <tap> <step>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position\n" +
                            "   step - The number of steps");
                break;
                case "encrypt": 
                    if(args.Length == 2)
                        Console.WriteLine($"The ciphertext is: {bits_to_string(encrypt_decrypt(getBits(args[1])))}");
                     else 
                        Console.WriteLine("Usage: Encrypt <plaintext>\n" +
                            "   plaintext - The plaintext in bits");
                break;
                case "decrypt":
                    if(args.Length == 2)
                        Console.WriteLine($"The plaintext is: {bits_to_string(encrypt_decrypt(getBits(args[1])))}");
                     else 
                        Console.WriteLine("Usage: Decrypt <plaintext>\n" +
                            "   plaintext - The plaintext in bits");
                break;
                case "triplebits": 
                    if(args.Length == 5 &&  int.TryParse(args[2], out tap) && int.TryParse(args[3], out step) 
                        && step > 0 && int.TryParse(args[4], out int iteration) && iteration > 0) {
                            triplebits(getBits(args[1]), tap, step, iteration);
                    } else 
                        Console.WriteLine("Usage: TripleBit <seed> <tap> <step> <iteration>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - The tap position\n" +
                            "   step - The number of steps\n" +
                            "   iteration - The number of iterations");
                break;
                case "encryptimage": break;
                case "decryptimage": break;
                default: 
                    Console.WriteLine("That option does not exist. Try one of " + 
                    "the following: \nCypher, GenerateKeystream, Encrypt, Decrypt, " + 
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
 
        static (int[] bits, int right_most) cipher(int[] bits, int tap) {

            var new_bit = bits[tap] ^ bits[0];

            for(int i = 0; i<bits.Length-1; i++) {
                bits[i] = bits[i+1];
            }

            bits[bits.Length-1] = new_bit;

            Console.WriteLine($"{bits_to_string(bits)} {new_bit}");
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
            using(StreamReader file = new StreamReader("keystream.txt")) {
                var keystream_string = file.ReadLine()+"";
                keystream = getBits(keystream_string);
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

        static void triplebits(int[] seed, int tap, int step, int iteration) {
            
        }

        static void encryptimage(string image_file, string seed, string tap) {

        }

        static void decryptimage(string image_file, string seed, string tap) {

        }
    }
}