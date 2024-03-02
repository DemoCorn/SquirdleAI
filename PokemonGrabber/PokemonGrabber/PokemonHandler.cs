using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using PokemonGrabber;

namespace PokemonGrabber
{
    public class PokemonHandler : List<Pokemon>
    {

        public PokemonHandler()
        {
        }

        public PokemonHandler(PokemonHandler ToCopy)
        {
            foreach (Pokemon Copy in ToCopy)
            {
                Add(Copy);
            }
        }

        public async Task asyncLoad()
        {
            // Check if directory exists
            if (Directory.Exists("PocketDex/Pokemon"))
            {
                
            }
            else
            {
                Task t = Getlist();
                await t;
            }
        }

        public async Task Getlist()
        {
            // Create Pokemon Basic
            BasicPokemonHandler PokemonBasics = new BasicPokemonHandler();

            var httpClient = HttpClientFactory.Create();

            try
            {
                // Call the data for all pokemon (this is hard coded, but faster on first load)
                string data = await httpClient.GetStringAsync("https://pokeapi.co/api/v2/pokemon?limit=1118");

                PokemonBasics = JsonConvert.DeserializeObject<BasicPokemonHandler>(data);
            }
            catch
            {
                Console.WriteLine("Failed to grab all basic pokemon");
            }

            foreach (BasicPokemon PKmon in PokemonBasics.results)
            {
                Task t = default(Task);
                try
                {
                    // Call api for all pokemon in the pokemonbasic
                    string data = await httpClient.GetStringAsync(PKmon.url);

                    Add(JsonConvert.DeserializeObject<Pokemon>(data));
                    await t;
                }
                catch
                {
                    Console.WriteLine("Failed to grab pokemon: " + PKmon.name);
                }
            }

            List<int> toRemove = new List<int>();
            List<int> IDSpeciesSkip = new List<int>();

            foreach (Pokemon PKmon in this)
            {
                string speciesURL = PKmon.species.url;
                int speciesNumber = -1;

                // Hardcoded but functional, for something like this the hardcoding is worth the massive amount of time it saves
                int lastSlashIndex = 41;

                // Grab the number at the end of the URL
                string numberString = speciesURL.Substring(lastSlashIndex + 1);
                numberString = numberString.Remove(numberString.Length - 1, 1);

                speciesNumber = int.Parse(numberString);

                if (PKmon.id != speciesNumber)
                {

                    if (speciesNumber > Count)
                    {
                        Console.WriteLine("Found species outside acceptable range while evaluating: " + PKmon.name);
                    }

                    if (
                        PKmon.types  == this[speciesNumber].types  &&
                        PKmon.weight == this[speciesNumber].weight &&
                        PKmon.height == this[speciesNumber].height
                        )
                    {
                        // Placing the ids to remove in backwards allows for not needing to do anything special when we actually remove them
                        toRemove.Insert(0, PKmon.id);
                    }
                    else
                    {
                        IDSpeciesSkip.Add(PKmon.id);
                    }
                }
            }

            // NOTE: AFTER THIS POINT ID DOES NOT CORRISPOND TO PLACE IN LIST
            foreach (int id in toRemove) 
            {
                RemoveAt(id);
            }


            //TODO: change this to take in species data and skip all pokemon with id within IDSpeciesSkip
            foreach (Pokemon PKmon in this)
            {
                Task t = default(Task);
                try
                {
                    // Call api for all pokemon in the pokemonbasic
                    string data = await httpClient.GetStringAsync(PKmon.species.url);

                    Add(JsonConvert.DeserializeObject<Pokemon>(data));
                    await t;
                }
                catch
                {
                    Console.WriteLine("Failed to grab pokemon: " + PKmon.name);
                }
            }

            Save();
        }
        private void Save()
        {
            // Create needed directories
            Directory.CreateDirectory("Data");
            Directory.CreateDirectory("Data/Pokemon");

            foreach (Pokemon PKmon in this)
            {
                try
                {
                    // Make every entry into files
                    using (StreamWriter writer = new StreamWriter("Data/Pokemon/" + PKmon.name + ".json"))
                    {
                        writer.Write(System.Text.Json.JsonSerializer.Serialize<Pokemon>(PKmon));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static bool ContainsChar(BasicPokemon word)
        {
            if (word.name.Contains("-gmax")
                || word.name.Contains("-mega")
                || word.name.Contains("-totem")
                || word.name.Contains("-eternamax")
                || word.name.Contains("-eternamax")
                || word.name.Contains("-own-tempo")
                || (word.name.Contains("minior") && !word.name.Contains("red"))
                || word.name.Contains("-ash")
                || word.name.Contains("-battle-bond")
                || word.name.Contains("-busted")
                || word.name.Contains("-resolute")
                || word.name.Contains("pikachu-")
                || word.name.Contains("-small")
                || word.name.Contains("-large")
                || word.name.Contains("-super")
                || word.name.Contains("-red-striped")
                )
            {
                return true;
            }
            return false;
        }

        public PokemonHandler GetOfType(string type1, string type2 = "None")
        {
            PokemonHandler HandlerToReturn = new PokemonHandler();

            // Find all pokemon with given types
            foreach (Pokemon PKmon in this)
            {
                if (PKmon.HasType(type1) && PKmon.HasType(type2))
                {
                    HandlerToReturn.Add(PKmon);
                }
            }

            return HandlerToReturn;
        }

        public PokemonHandler PokemonContainsName(string name)
        {
            PokemonHandler HandlerToReturn = new PokemonHandler();

            name = name.ToLower();

            // Find all pokemon with given characters in the name
            foreach (Pokemon PKmon in this)
            {
                if (PKmon.name.Contains(name))
                {
                    HandlerToReturn.Add(PKmon);
                }
            }

            return HandlerToReturn;
        }
    }
}

