using StrawPollNET.Enums;
using System.Collections.Generic;

namespace SuperBot_2_0.Services
{
    public class Poll
    {
        public int Id { get; internal set; }
        public string Title { get; internal set; }
        public List<string> Options { get; internal set; }
        public List<int> Votes { get; internal set; }
        public bool Multi { get; internal set; }
        internal string Dupcheck { get; set; }

        public DupCheck DupCheck
        {
            get
            {
                switch (Dupcheck)
                {
                    case "normal":
                        return DupCheck.Normal;

                    case "permissive":
                        return DupCheck.Permissive;

                    default:
                        return DupCheck.Normal;
                }
            }
        }

        public bool Captcha { get; internal set; }

        public string PollUrl => $"https://strawpoll.me/{Id}";
    }

    public static class Extension
    {
        public static DupCheck ToDupCheck(this string str)
        {
            switch (str)
            {
                case "normal":
                    return DupCheck.Normal;

                case "permissive":
                    return DupCheck.Permissive;

                default:
                    return DupCheck.Normal;
            }
        }
    }
}