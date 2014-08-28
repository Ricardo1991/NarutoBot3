NarutoBot3
=========

Simple private IRC bot made with Windows Forms

To run, it needs the following files in the same dir as the .exe

* help.txt      - Contains the text for the !help command
* ops.txt       - Contains the nicks that can operate the bot
* rules.txt     - Contains the text for the !rules command
* banned.txt    - Contains the nicks that are ignored by the bot
* text.txt      - Strings to be used by the nick generator, !nick
* trivia.txt    - Trivia to be used on the !trivia command
* kills.txt     - A file with kill strings. On the strings, <target> is the person that dies, <user> is the one that ordered the kill

It requires RedditSharp - https://github.com/SirCmpwn/RedditSharp
