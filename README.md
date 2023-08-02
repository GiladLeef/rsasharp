# LES Algorithm

The LES (Leef Encryption System) is a simple implementation of the RSA algorithm. It allows users to generate key pairs, encrypt plaintext, and decrypt ciphertext.

## Usage

The program can be run with the following command-line arguments:

1. `generate` - Generates a new key pair and writes the public and private keys to files.
2. `encrypt <public-key-file> <input-file> <output-file>` - Encrypts the input file using the specified public key file and writes the ciphertext to the output file.
3. `decrypt <private-key-file> <input-file> <output-file>` - Decrypts the input file using the specified private key file and writes the plaintext to the output file.

## Prerequisites

This program requires the .NET framework and a C# compiler to run.

## Generating Key Pair

To generate a new key pair, run the program with the `generate` argument:

`LES.exe generate`

This will generate a new pair of public and private keys and write them to `public.key` and `private.key` files, respectively. The generated keys will be displayed on the console as well.

## Encryption

To encrypt a file using a public key, run the program with the `encrypt` argument followed by the public key file, input file, and output file paths:

`LES.exe encrypt <public-key-file> <input-file> <output-file>`

For example:

`LES encrypt public.key plaintext.txt ciphertext.txt`


This will read the public key from `public.key`, encrypt the contents of `plaintext.txt`, and write the ciphertext to `ciphertext.txt`.

## Decryption

To decrypt a file using a private key, run the program with the `decrypt` argument followed by the private key file, input file, and output file paths:

`LES.exe decrypt <private-key-file> <input-file> <output-file>`


For example:

`LES.exe decrypt private.key ciphertext.txt plaintext.txt`


This will read the private key from `private.key`, decrypt the ciphertext from `ciphertext.txt`, and write the plaintext to `plaintext.txt`.

## Security

Please note that this implementation is intended for educational purposes and may not provide the same level of security as production-grade encryption systems. Use it at your own risk.

## License

This code is provided under the [MIT License](https://opensource.org/licenses/MIT).
