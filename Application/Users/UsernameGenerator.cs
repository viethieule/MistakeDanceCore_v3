using System.Text.RegularExpressions;
using Application.Common.Helpers;
using Application.Common.Interfaces;

namespace Application.Users
{
    public class UsernameGenerator : IUsernameGenerator
    {
        private readonly IUserService _userService;

        public UsernameGenerator(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<string> Generate(string normalizedFullName)
        {
            if (string.IsNullOrWhiteSpace(normalizedFullName))
            {
                return string.Empty;
            }

            string[] names = Regex.Split(normalizedFullName.ToLower(), @"\s+");

            if (names.Length == 1)
            {
                string userName = names[0];

                List<string> existingUsernames = await _userService.GetUsernamesStartWith(userName);
                int numberOfExistingUsernames = existingUsernames.Count(u =>
                {
                    string[] part = u.Split('.');
                    return part[0] == userName &&
                        (part.Length == 1 || (part.Length == 2 && int.TryParse(part[1], out _)));
                });

                if (numberOfExistingUsernames == 0)
                {
                    return userName;
                }
                else
                {
                    return userName + "." + numberOfExistingUsernames;
                }
            }
            else
            {
                string userName = names[names.Length - 1] + "." + names[0];

                List<string> existingUsernames = await _userService.GetUsernamesStartWith(userName);
                int numberOfExistingUsernames = existingUsernames.Count(u =>
                {
                    string[] part = u.Split('.');
                    return part[0] + "." + part[1] == userName;
                });

                if (numberOfExistingUsernames == 0)
                {
                    return userName;
                }
                else
                {
                    return userName + "." + numberOfExistingUsernames;
                }
            }
        }
    }
}