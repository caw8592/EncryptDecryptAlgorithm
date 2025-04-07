

namespace Project {
    class Project {
        static void Main(string[] args) {
            switch(args[0].ToLower()) {
                case "cipher": 
                    if(args.Length != 3)
                        Console.WriteLine("Usage: Cipher <seed> <tap>\n" +
                            "   seed - The initial seed\n" +
                            "   tap - the tap position");
                    else 
                        cipher(args[1], Convert.ToInt32(args[2])); 
                break;
                case "generatekeystream": break;
                case "encrypt": break;
                case "decrypt": break;
                case "triplebits": break;
                case "encryptimage": break;
                case "decryptimage": break;
                default: 
                    Console.WriteLine("That option does not exist. Try one of " + 
                    "the following: \nCypher, GenerateKeystream, Encrypt, Decrypt, " + 
                    "TripleBit, EncryptImage, DecryptImage");
                break;
            }
        }

        static void cipher(string seed, int tap) {
            Console.WriteLine($"{seed} - seed");

            var bits = new int[seed.Length];
            for(int i = 0; i<seed.Length; i++) {
                
            }


            var new_bit = bytes[tap-1] ^ bytes[bytes.Length-1];

            for(int i = bytes.Length-1; i>0; i--) {
                bytes[i] = bytes[i-1];
            }

            bytes[0] = new_bit[0];

            Console.WriteLine($"{bytes} {new_bit}");
        }

        static void generatekeystream(string seed, int tap, string step) {
        }

        static void encrypt(string text) {

        }

        static void decrypt(string text) {

        }

        static void triplebits(string seed, string tap, string step, string iteration) {

        }

        static void encryptimage(string image_file, string seed, string tap) {

        }

        static void decryptimage(string image_file, string seed, string tap) {

        }
    }
}