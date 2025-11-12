using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingApp.DataStructures
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; set; }
        public bool IsEndOfWord { get; set; }
        public int Frequency { get; set; }
        public string Word { get; set; }

        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode>();
            IsEndOfWord = false;
            Frequency = 0;
            Word = string.Empty;
        }
    }

    public class CustomTrie
    {
        private TrieNode root;

        public CustomTrie()
        {
            root = new TrieNode();
        }

        public void Insert(string word, int frequency = 1)
        {
            if (string.IsNullOrWhiteSpace(word)) return;

            word = word.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                {
                    current.Children[c] = new TrieNode();
                }
                current = current.Children[c];
            }

            current.IsEndOfWord = true;
            current.Frequency += frequency;
            current.Word = word;
        }

        public bool Search(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return false;

            word = word.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                    return false;
                current = current.Children[c];
            }

            return current.IsEndOfWord;
        }

        public bool StartsWith(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return false;

            prefix = prefix.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in prefix)
            {
                if (!current.Children.ContainsKey(c))
                    return false;
                current = current.Children[c];
            }

            return true;
        }

        public List<string> GetSuggestions(string prefix, int maxSuggestions = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return new List<string>();

            prefix = prefix.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in prefix)
            {
                if (!current.Children.ContainsKey(c))
                    return new List<string>();
                current = current.Children[c];
            }

            var suggestions = new List<(string word, int frequency)>();
            CollectWords(current, suggestions);

            return suggestions
                .OrderByDescending(s => s.frequency)
                .ThenBy(s => s.word)
                .Take(maxSuggestions)
                .Select(s => s.word)
                .ToList();
        }

        private void CollectWords(TrieNode node, List<(string word, int frequency)> words)
        {
            if (node.IsEndOfWord)
            {
                words.Add((node.Word, node.Frequency));
            }

            foreach (var child in node.Children.Values)
            {
                CollectWords(child, words);
            }
        }

        public List<string> GetMostFrequentWords(int count = 10)
        {
            var allWords = new List<(string word, int frequency)>();
            CollectWords(root, allWords);

            return allWords
                .OrderByDescending(w => w.frequency)
                .Take(count)
                .Select(w => w.word)
                .ToList();
        }

        public void UpdateFrequency(string word, int additionalFrequency = 1)
        {
            if (string.IsNullOrWhiteSpace(word)) return;

            word = word.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                    return;
                current = current.Children[c];
            }

            if (current.IsEndOfWord)
            {
                current.Frequency += additionalFrequency;
            }
        }

        public int GetWordFrequency(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return 0;

            word = word.ToLower().Trim();
            TrieNode current = root;

            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                    return 0;
                current = current.Children[c];
            }

            return current.IsEndOfWord ? current.Frequency : 0;
        }

        public void Clear()
        {
            root = new TrieNode();
        }

        public int GetTotalWords()
        {
            var allWords = new List<(string word, int frequency)>();
            CollectWords(root, allWords);
            return allWords.Count;
        }

        public List<string> GetFuzzySuggestions(string input, int maxDistance = 2, int maxSuggestions = 5)
        {
            if (string.IsNullOrWhiteSpace(input)) return new List<string>();

            input = input.ToLower().Trim();
            var allWords = new List<(string word, int frequency)>();
            CollectWords(root, allWords);

            var suggestions = allWords
                .Where(w => CalculateEditDistance(input, w.word) <= maxDistance)
                .OrderBy(w => CalculateEditDistance(input, w.word))
                .ThenByDescending(w => w.frequency)
                .Take(maxSuggestions)
                .Select(w => w.word)
                .ToList();

            return suggestions;
        }

        private int CalculateEditDistance(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                dp[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                        dp[i, j] = 1 + Math.Min(Math.Min(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1]);
                }
            }

            return dp[s1.Length, s2.Length];
        }
    }
}