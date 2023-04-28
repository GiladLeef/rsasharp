using System.Numerics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "generate")
        {
            GenerateKeyPair();
            return;
        }

        if (args.Length == 1 && args[0] == "decrypt")
        {
            Console.WriteLine("Decrypting Usage: LES decrypt <private-key-file> <input-file> <output-file>");
            return;
        }

        if (args.Length == 4 && args[0] == "decrypt")
        {
            string privateKeyFile = args[1];
            string decryptInputFile = args[2];
            string decryptOutputFile = args[3];
            Decrypt(privateKeyFile, decryptInputFile, decryptOutputFile);
            return;
        }

        if (args.Length == 1 && args[0] == "encrypt")
        {
            Console.WriteLine("Encrypting Usage: LES encrypt <public-key-file> <input-file> <output-file>");
            return;
        }

        if (args.Length == 4 && args[0] == "encrypt")
        {
            // Read public key from file
            string publicKeyFile = args[1];
            string publicKeyText = File.ReadAllText(publicKeyFile);
            string[] publicKeyLines = publicKeyText.Split(' ');
            BigInteger e = BigInteger.Parse(publicKeyLines[0].Trim());
            BigInteger n = BigInteger.Parse(publicKeyLines[1].Trim());

            // Read input from file
            string inputFile = args[2];
            string inputText = File.ReadAllText(inputFile);

            // Encrypt input using RSA
            BigInteger[] ciphertexts = Encrypt(inputText, e, n);

            // Write ciphertext to file
            string outputFile = args[3];
            using (StreamWriter sw = File.CreateText(outputFile))
            {
                foreach (var ciphertext in ciphertexts)
                {
                    sw.Write("{0} ", ciphertext);
                }
            }
            return;
        }

        Console.WriteLine("Usage: LES [generate] [encrypt] [decrypt]");
    }

static void GenerateKeyPair()
{
    BigInteger p = GetPrime();
    BigInteger q = GetPrime();
    BigInteger n = p * q;
    BigInteger phi = (p - 1) * (q - 1);
    BigInteger e = 65537;
    BigInteger d = ModInverse(e, phi);

    // Write public key to file
    using (StreamWriter file = new StreamWriter("public.key"))
    {
        file.WriteLine("{0} {1}", e, n);
    }

    // Write private key to file
    using (StreamWriter file = new StreamWriter("private.key"))
    {
        file.WriteLine("{0} {1}", d, n);
    }

    Console.WriteLine("Public key: {0} {1}", e, n);
    Console.WriteLine("Private key: {0} {1}", d, n);
}

    static BigInteger GetPrime()
    {
        // Generate a random 2048-bit number
        Random random = new Random();
        BigInteger n = BigInteger.Pow(2, 2048) + random.Next(0, int.MaxValue);

        // Test if number is prime
        while (!IsPrime(n))
        {
            n++;
        }

        return n;
    }

    static bool IsPrime(BigInteger n)
    {
        // Check if n is even
        if (n % 2 == 0)
        {
            return false;
        }

        // Test if n is prime using Fermat's little theorem
        for (int i = 0; i < 100; i++)
        {
            BigInteger a = RandomBigInteger(n - 2) + 1;
            if (ModPow(a, n - 1, n) != 1)
            {
                return false;
            }
        }

        return true;
    }

    static BigInteger RandomBigInteger(BigInteger max)
    {
        // Generate a random BigInteger less than max
        Random random = new Random();
        byte[] bytes = max.ToByteArray();
        random.NextBytes(bytes);
        bytes[bytes.Length - 1] &= (byte)0x7F;
        return new BigInteger(bytes) % max;
    }

    static BigInteger ModPow(BigInteger a, BigInteger b, BigInteger n)
    {
        // Compute a^b mod n using repeated squaring
        BigInteger result = 1;
        while (b > 0)
        {
            if (b % 2 == 1)
            {
                result = (result * a) % n;
            }
            a = (a * a) % n;
            b /= 2;
        }
        return result;
    }

    static BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        // Compute the modular inverse of a mod n using the extended Euclidean algorithm
        BigInteger t = 0, newt = 1, r = n, newr = a;
        while (newr != 0)
        {
            BigInteger quotient = r / newr;
            BigInteger temp = t;
            t = newt;
            newt = temp - quotient * newt;
            temp = r;
            r = newr;
            newr = temp - quotient * newr;
        }
        if (r > 1)
        {
            throw new Exception("a is not invertible");
        }
        if (t < 0)
        {
            t += n;
        }
        return t;
    }

static BigInteger[] Encrypt(string plaintext, BigInteger e, BigInteger n)
{
    // Convert plaintext to byte array
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(plaintext);

    // Split plaintext into chunks of 512 bytes
    int chunkSize = 512;
    int numChunks = (bytes.Length + chunkSize - 1) / chunkSize;
    byte[][] chunks = new byte[numChunks][];
    for (int i = 0; i < numChunks; i++)
    {
        int size = Math.Min(chunkSize, bytes.Length - i * chunkSize);
        chunks[i] = new byte[size];
        Array.Copy(bytes, i * chunkSize, chunks[i], 0, size);
    }

    // Encrypt each chunk using RSA
    BigInteger[] ciphertexts = new BigInteger[numChunks];
    for (int i = 0; i < numChunks; i++)
    {
        BigInteger m = new BigInteger(chunks[i]);
        ciphertexts[i] = ModPow(m, e, n);
    }

    return ciphertexts;
}



    static void Decrypt(string privateKeyFile, string inputFile, string outputFile)
    {
        // Read private key from file
        string privateKeyText = File.ReadAllText(privateKeyFile);
        string[] privateKeyLines = privateKeyText.Split(' ');
        BigInteger d = BigInteger.Parse(privateKeyLines[0].Trim());
        BigInteger n = BigInteger.Parse(privateKeyLines[1].Trim());

        // Read ciphertexts from file
        string ciphertextText = File.ReadAllText(inputFile);
        string[] ciphertextLines = ciphertextText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        BigInteger[] ciphertexts = new BigInteger[ciphertextLines.Length];
        for (int i = 0; i < ciphertextLines.Length; i++)
        {
            ciphertexts[i] = BigInteger.Parse(ciphertextLines[i].Trim());
        }

        // Decrypt each ciphertext using RSA
        byte[][] plaintextChunks = new byte[ciphertexts.Length][];
        for (int i = 0; i < ciphertexts.Length; i++)
        {
            BigInteger c = ciphertexts[i];
            BigInteger m = ModPow(c, d, n);
            plaintextChunks[i] = m.ToByteArray();
        }

        // Concatenate plaintext chunks and write to output file
        byte[] plaintextBytes = plaintextChunks.SelectMany(x => x).ToArray();
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);
        File.WriteAllText(outputFile, plaintext);
    }    
}