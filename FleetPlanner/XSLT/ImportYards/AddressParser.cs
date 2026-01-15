using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tfp.Datamodel;

namespace ImportYards
{
    public class AddressParser
    {
        public static void ParseStreet(string streetline, Address address)
        {

            if (string.IsNullOrEmpty(streetline))
            {
                return;
            }

            streetline = streetline.Trim();

            // Regex: ^(\d+) matches leading digits, \s+ is space, (.+) is the rest
            Match match = Regex.Match(streetline, @"^(\d+)\s+(.+)$");

            if (match.Success)
            {
                address.HouseNo = match.Groups[1].Value;
                address.Street = match.Groups[2].Value;
            }
            else
            {
                address.Street = streetline;
            }
            return;
        }
    }
}
