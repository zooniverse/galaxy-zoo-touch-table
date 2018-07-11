using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Models
{
    enum UserType
    {
        Person,
        Star,
        Earth,
        Light,
        Face,
        Heart
    }

    abstract class TableUser
    {
        public abstract string ThemeColor { get; }
        public abstract string Avatar { get; }
    }

    class PersonUser : TableUser
    {
        public override string ThemeColor => "#A5A2FB";
        public override string Avatar => "Avatar";
    }

    class StarUser : TableUser
    {
        public override string ThemeColor => "#29A1FA";
        public override string Avatar => "Avatar";
    }

    class EarthUser : TableUser
    {
        public override string ThemeColor => "#6ADCA3";
        public override string Avatar => "Avatar";
    }

    class LightUser : TableUser
    {
        public override string ThemeColor => "#A3DDEE";
        public override string Avatar => "Avatar";
    }

    class FaceUser : TableUser
    {
        public override string ThemeColor => "#F3AB91";
        public override string Avatar => "Avatar";
    }

    class HeartUser : TableUser
    {
        public override string ThemeColor => "#F3588B";
        public override string Avatar => "Avatar";
    }

    static class TableUserFactory
    {
        public static TableUser Create(UserType type)
        {
            switch (type)
            {
                case UserType.Person:
                    return new PersonUser();
                case UserType.Star:
                    return new StarUser();
                case UserType.Earth:
                    return new EarthUser();
                case UserType.Light:
                    return new LightUser();
                case UserType.Face:
                    return new FaceUser();
                case UserType.Heart:
                    return new HeartUser();
                default:
                    return new HeartUser();

            }
        }
    }
}
