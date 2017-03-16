﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Content
{
    /// <summary>
    /// String in international FFG format. Including all available languages supported by MoM App
    /// </summary>
    public class StringI18n
    {

        private DictionaryI18n referedDictionary;

        /// <summary>
        /// Instance info of the current translations
        /// </summary>
        private string[] translations;
       
        /// <summary>
        /// Creates an empty instance of a Multilanguage String
        /// </summary>
        public StringI18n(DictionaryI18n dict)
        {
            referedDictionary = dict;
            translations = new string[dict.getLanguages().Length];
        }

        private const char QUOTES = '\"';
        private const char COMMA = ',';

        /// <summary>
        /// Constructor with the complete localisation elements
        /// </summary>
        /// <param name="completeLocalisationString"></param>
        public StringI18n(DictionaryI18n dict,string completeLocalisationString)
        {
            referedDictionary = dict;

            if (completeLocalisationString.Contains(QUOTES))
            {
                // with quotes, commas inside quotes isn't considered separator
                List<string> partialTranslation = new List<string>(completeLocalisationString.Split(COMMA));
                List<string> finalTranslation = new List<string>();
                string currentTranslation = "";
                bool opened = false;
                foreach (string suposedTranslation in partialTranslation)
                {
                    currentTranslation += suposedTranslation;

                    bool initialQuote = suposedTranslation.Length != 0 && suposedTranslation[0] == QUOTES;
                    bool finalQuote = suposedTranslation.Length > 1 &&
                        suposedTranslation[suposedTranslation.Length - 1] == QUOTES;

                    // If contains one quote we need to analyze
                    if (initialQuote ^ finalQuote)
                    {
                        if (opened)
                        {
                            // Closing quotes
                            finalTranslation.Add(
                                // remove initial and final quote
                                currentTranslation.Substring(1, currentTranslation.Length - 2)
                                // replace double quotes for single quotes
                                .Replace("\"\"", "\"")
                                );
                            currentTranslation = "";
                        }
                        else
                        {
                            currentTranslation += COMMA;
                        }
                        opened = !opened;
                    }
                    else
                    {
                        if (initialQuote)
                        {
                            currentTranslation = currentTranslation.Substring(1, currentTranslation.Length - 2);
                        }
                        // other options are no quotes
                        // both need same proceed.
                        finalTranslation.Add(currentTranslation.Replace("\"\"", "\""));
                        currentTranslation = "";
                    }
                }
                translations = finalTranslation.ToArray();
            }
            else
            {
                // Without quotes, all commas are separators
                translations = completeLocalisationString.Split(COMMA);
            }

            if (translations.Length != dict.getLanguages().Length)
            {
                Debug.Log("Incoherent DictI18n with " + dict.getLanguages().Length + " languages including StringI18n: " + completeLocalisationString + System.Environment.NewLine);
            }
        }

        // The key is que position 0 of the array
        public string key
        {
            get
            {
                return translations[0];
            }
        }

        public string getSpecificLanguageString(int nLanguage)
        {
            return translations[nLanguage];
        }

        /// <summary>
        /// In translation of texts. If we don't have current language text, a
        /// specific language text will be got. In order to know if there is a 
        /// current language text the method HasTextInCurrentLanguage can be used.
        /// </summary>
        /// <param name="nLanguage">number of the language to use</param>
        /// <returns></returns>
        public string getCurrentOrDefaultLanguageString()
        {
            if (HasTextInCurrentLanguage)
            {
                return currentLanguageString;
            } else
            {
                return getSpecificLanguageString(referedDictionary.defaultLanguage);
            }            
        }

        /// <summary>
        /// The string value of the key whith the current language
        /// </summary>
        public string currentLanguageString
        {
            get
            {
                return translations[referedDictionary.currentLanguage];
            }
            set
            {
                translations[referedDictionary.currentLanguage] = value;
            }
        }

        public bool HasTextInCurrentLanguage
        {
            get
            {
                return currentLanguageString.Length > 0;
            }
        }

        /// <summary>
        /// String representation of the multilanguage element
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            bool first = true;
            foreach (string oneTranslation in translations)
            {
                if (!first)
                {
                    result.Append(COMMA);
                }

                if (oneTranslation.Contains(COMMA) || oneTranslation.Contains(QUOTES))
                {
                    // The serializable text should repeat mid quotes and add initial and final quotes
                    result.Append(QUOTES).Append(oneTranslation.Replace(QUOTES.ToString(),"\"\"")).Append(QUOTES);
                }
                else
                {
                    result.Append(oneTranslation);
                }

                if (first)
                {
                    first = false;
                }
            }

            return result.ToString();
        }
    }
}