﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool shop;

        short jobs;

        public UserAttribute() : this(false, 0) { }

        public UserAttribute(bool shop, short jobs)
        {
            this.shop = shop;
            this.jobs = jobs;
        }

        public bool Shop => shop;

        public short Jobs => jobs;

        protected internal override AuthorizeAttribute Clone()
        {
            return new UserAttribute(shop, jobs);
        }
        protected internal override void Or(AuthorizeAttribute another)
        {
            var ua = another as UserAttribute;
            shop |= ua.shop;
            jobs |= ua.jobs;
        }

        protected internal override bool Check(IData token)
        {
            User tok = token as User;

            if (tok == null) return false;

            if (tok.shopid == null)
            {
                return false;
            }
            if (tok == null) return false;

            if (jobs != 0 && (tok.jobs & jobs) == 0)
            {
                return false;
            }
            return true;
        }
    }
}