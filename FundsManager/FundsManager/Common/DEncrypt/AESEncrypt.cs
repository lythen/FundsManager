﻿using System;
        private static string _password = "ZJZVR2VJpm9CYcXV7y62hKhbjLXMnm2qUs5hRimjCDFxTSt6zZTKnwzpCXJe5h5bKsGF8QcxlwQ4fHjq30xpvGc9Uq5SyiE5hDE9mCX7W29T6SjVN3BeHqGcpgKqujac";
        public static string Encrypt(string text)
        {
            return Encrypt(text, _password, _iv);
        }
        public static string Decrypt(string text)
        {
            return Decrypt(text, _password, _iv);
        }
        /// <summary>
        public static string Encrypt(string text, string password, string iv)
        /// <summary>
        /// <summary>