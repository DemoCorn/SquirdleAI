using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace PokemonGrabber
{
    public class Pokemon
    {
        public List<Ability> abilities { get; set; }
        public int base_experience { get; set; }
        public List<Form> forms { get; set; }
        public List<GameIndice> game_indices { get; set; }
        public int height { get; set; }
        public List<HeldItem> held_items { get; set; }
        public int id { get; set; }
        public bool is_default { get; set; }
        public string location_area_encounters { get; set; }
        public List<Move> moves { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public List<Stat> past_types { get; set; }
        public Species species { get; set; }
        public Sprites sprites { get; set; }
        public List<Stat> stats { get; set; }
        public List<Type> types { get; set; }
        public int weight { get; set; }

        public bool HasType(string TypeToTest)
        {
            TypeToTest = TypeToTest.ToLower();
            if (TypeToTest == "none")
            {
                return true;
            }
            // Go through 1-2 types, if neithers name are equal to the given type it will return true
            foreach (Type TypeTester in types)
            {
                if (TypeTester.type.name == TypeToTest)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
