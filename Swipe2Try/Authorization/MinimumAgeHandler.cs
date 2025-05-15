using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Swipe2Try.Authorization
{
    // Example requirement for a more complex authorization rule
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }

    // Handler for the MinimumAgeRequirement
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            MinimumAgeRequirement requirement)
        {
            // In a real app, you would check the user's age from a claim or database
            // For demo purposes, we'll just check for a specific role
            if (context.User.IsInRole("ADMIN"))
            {
                // Automatically succeed for admins
                context.Succeed(requirement);
            }

            // In a real app, you would check:
            // var dateOfBirthClaim = context.User.FindFirst(c => c.Type == "DateOfBirth");
            // if (dateOfBirthClaim != null)
            // {
            //     // Parse the DOB and check age...
            //     // If age >= requirement.MinimumAge, then:
            //     // context.Succeed(requirement);
            // }

            return Task.CompletedTask;
        }
    }
}
