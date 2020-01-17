using System.Collections.Generic;

namespace Statens.Tribes.App.Domain.Model
{
    public class TribeMember
    {
        public string MemberKey { get; set; }

        public string DisplayName { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class TribeMembershipList
    {
        public string MemberKey { get; set; }

        public List<TribeMembership> Memberships { get; set; }
    }

    public class TribeMembership
    {
        public string TribeKey { get; set; }

        public string TribeName { get; set; }

        public string TribeRole { get; set; }

        public TribeType Type { get; set; }
    }
}