# Leef Encryption System
A complete implementation of the RSA algorithm including a CLI tool in pure C#.
Basically an encryption algorithm secured by the fact it's "hard" to factor semi-prime numbers.

note that uses 4096 bit keys by default. since it's able to encrypt messages only upto 4096 bits of data, it will check and if the message is more than 4096 bits, it will split it into chunks and encrypt them one by one.

Documented using ChatGPT.
