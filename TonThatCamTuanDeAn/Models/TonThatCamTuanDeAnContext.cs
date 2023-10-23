﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TonThatCamTuanDeAn.Models;

public partial class TonThatCamTuanDeAnContext : DbContext
{
    public TonThatCamTuanDeAnContext()
    {
    }

    public TonThatCamTuanDeAnContext(DbContextOptions<TonThatCamTuanDeAnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=connectString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
