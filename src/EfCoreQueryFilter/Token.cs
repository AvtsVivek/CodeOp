using System;

namespace MultiTenant
{
    public record Token(Guid TenantId);
}