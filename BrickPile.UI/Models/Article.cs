﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using BrickPile.Domain;
using BrickPile.Domain.Models;
using BrickPile.Samples.Controllers;

namespace BrickPile.Samples.Models {
    [ContentType(ControllerType = typeof(ArticleController),Name = "Article")]
    public class Article : IContent {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string Heading { get; set; }

        [DataType(DataType.MultilineText)]
        public string MainIntro { get; set; }

        [UIHint("Markdown")]
        public string FireAtWill { get; set; }

        public int Count { get; set; }
        
        public Contact Contact { get; set; }
    }
    public class ArticleViewModel {
        public Article Content { get; set; }
        public IPageModel CurrentPage { get; set; }
    }

    public class Contact {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
    public static class StringExtensions {
        public static string ToCamelCase(this string input) {

            if (string.IsNullOrEmpty(input))
                return input;

            string[] splitted = input.Split('.');
            var list = new List<string>(splitted.Length);
            
            foreach (string s in splitted) {

                if(!char.IsUpper(s[0])) {
                    list.Add(s);
                    continue;
                }

                string str = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString((IFormatProvider)CultureInfo.InvariantCulture);
                if (s.Length > 1)
                    str = str + s.Substring(1);
                list.Add(str);
            }
            return string.Join(".", list);
        }
    }
}