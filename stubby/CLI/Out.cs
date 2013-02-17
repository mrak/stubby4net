using System;

namespace stubby.CLI {

   internal static class Out {
      public static bool Mute = false;

      private static void Print(string message, ConsoleColor color) {
         if (Mute) return;

         Console.ForegroundColor = color;
         Console.WriteLine(message);
         Console.ResetColor();
      }

      public static void Linefeed() {
         if (!Mute) Console.WriteLine();
      }

      public static void Warn(string message) {
         Print(message, ConsoleColor.DarkYellow);
      }

      public static void Error(string message) {
         Print(message, ConsoleColor.DarkRed);
      }

      public static void Notice(string message) {
         Print(message, ConsoleColor.DarkMagenta);
      }

      public static void Status(string message) {
         Print(message, ConsoleColor.DarkGray);
      }

      public static void Incoming(string message) {
         Print(message, ConsoleColor.DarkCyan);
      }

      public static void Success(string message) {
         Print(message, ConsoleColor.DarkGreen);
      }

      public static void Info(string message) {
         Print(message, ConsoleColor.DarkBlue);
      }
   }

}