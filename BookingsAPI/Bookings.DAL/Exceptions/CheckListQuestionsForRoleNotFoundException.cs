using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class CheckListQuestionsForRoleNotFoundException : Exception
    {
        public CheckListQuestionsForRoleNotFoundException(string userRole) : base($"Questions for role {userRole} does not exist")
        {
        }
    }
}
