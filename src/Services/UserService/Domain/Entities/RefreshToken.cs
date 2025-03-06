﻿using SharedKernel.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class RefreshToken(Guid id, Guid userId, string token, DateTime expiryDate, string createdByIp)
    : Entity<Guid>(id)
{
    public Guid UserId { get; private set; } = userId;
    [ForeignKey("UserId")]
    public User User { get; private set; }

    public string Token { get; private set; } = token;
    public DateTime ExpiryDate { get; private set; } = expiryDate;
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string CreatedByIp { get; private set; } = createdByIp;

    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke()
    {
        RevokedAt = DateTime.Now;
    }

    public void Replace(string newToken)
    {
        ReplacedByToken = newToken;
    }
}
