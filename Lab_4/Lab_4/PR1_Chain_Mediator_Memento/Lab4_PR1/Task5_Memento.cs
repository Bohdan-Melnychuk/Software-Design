using System;
using System.Collections.Generic;

namespace Lab4_PR1.Task5
{
    public class TextDocument
    {
        public string Content { get; set; }

        public TextDocument()
        {
            Content = "";
        }

        public TextDocumentMemento Save()
        {
            return new TextDocumentMemento(Content);
        }

        public void Restore(TextDocumentMemento memento)
        {
            Content = memento.Content;
        }
    }

    public class TextDocumentMemento
    {
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        public TextDocumentMemento(string content)
        {
            Content = content;
            Timestamp = DateTime.Now;
        }
    }

    public class TextEditor
    {
        private TextDocument _document;
        private Stack<TextDocumentMemento> _history;

        public TextEditor()
        {
            _document = new TextDocument();
            _history = new Stack<TextDocumentMemento>();
        }

        public void Write(string text)
        {
            _history.Push(_document.Save());
            _document.Content += text;
            Console.WriteLine($"Додано: \"{text}\"");
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                _document.Restore(_history.Pop());
                Console.WriteLine("Відмінено останню дію.");
            }
            else
            {
                Console.WriteLine("Немає дій для скасування.");
            }
        }

        public void Show()
        {
            Console.WriteLine($"Поточний документ: \"{_document.Content}\"");
        }
    }

    public static class MementoDemo
    {
        public static void Run()
        {
            Console.WriteLine("\nЗавдання 5: Мементо (Memento)\n");

            var editor = new TextEditor();

            editor.Write("Hello ");
            editor.Write("World!");
            editor.Show();

            editor.Undo();
            editor.Show();

            editor.Write("C# Lab4");
            editor.Show();

            editor.Undo();
            editor.Show();
        }
    }
}