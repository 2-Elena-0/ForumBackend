using System;
using System.Collections.Generic;
using ForumBackend.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Ef;

public partial class ForumDbContext : DbContext
{
    public ForumDbContext()
    {
    }

    public ForumDbContext(DbContextOptions<ForumDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<InterestingTopic> InterestingTopics { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostsTopic> PostsTopics { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=Forum;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comments_pk");

            entity.ToTable("comments", "Forum");

            entity.HasIndex(e => e.Uid, "comments_unique").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Likes).HasColumnName("likes");
            entity.Property(e => e.Post).HasColumnName("post");
            entity.Property(e => e.ToComment).HasColumnName("to_comment");
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");
            entity.Property(e => e.WasDeleted).HasColumnName("was_deleted");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Comments)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_users_fk");

            entity.HasOne(d => d.PostNavigation).WithMany(p => p.Comments)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Post)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_posts_fk");

            entity.HasOne(d => d.ToCommentNavigation).WithMany(p => p.InverseToCommentNavigation)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.ToComment)
                .HasConstraintName("comments_comments_fk");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("favorites_pk");

            entity.ToTable("favorites", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Post).HasColumnName("post");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.PostNavigation).WithMany(p => p.FavoritesNavigation)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Post)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorites_posts_fk");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Favorites)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorites_users_fk");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("follows_pk");

            entity.ToTable("follows", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Follow1).HasColumnName("follow");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.Follow1Navigation).WithMany(p => p.FollowFollow1Navigations)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Follow1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("follows_users_fk_1");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.FollowUserNavigations)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("follows_users_fk");
        });

        modelBuilder.Entity<InterestingTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("interesting_topics_pk");

            entity.ToTable("interesting_topics", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Topic).HasColumnName("topic");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.TopicNavigation).WithMany(p => p.InterestingTopics)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Topic)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interesting_topics_topic_fk");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.InterestingTopics)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interesting_topics_users_fk");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("likes_pk");

            entity.ToTable("likes", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Post).HasColumnName("post");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.LikesNavigation)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("likes_posts_fk");

            entity.HasOne(d => d.User1).WithMany(p => p.Likes)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("likes_users_fk");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_pk");

            entity.ToTable("posts", "Forum");

            entity.HasIndex(e => e.Uid, "posts_unique").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Favorites).HasColumnName("favorites");
            entity.Property(e => e.Likes).HasColumnName("likes");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");
            entity.Property(e => e.UserDeleted).HasColumnName("user_deleted");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Posts)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_users_fk");
        });

        modelBuilder.Entity<PostsTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_topic_pk");

            entity.ToTable("posts_topic", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Post).HasColumnName("post");
            entity.Property(e => e.Topic).HasColumnName("topic");

            entity.HasOne(d => d.PostNavigation).WithMany(p => p.PostsTopics)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Post)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_topic_posts_fk");

            entity.HasOne(d => d.TopicNavigation).WithMany(p => p.PostsTopics)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Topic)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_topic_topic_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("role_pk");

            entity.ToTable("role", "Forum");

            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.CreateComment)
                .HasDefaultValue(true)
                .HasColumnName("create_comment");
            entity.Property(e => e.CreatePost)
                .HasDefaultValue(true)
                .HasColumnName("create_post");
            entity.Property(e => e.DeleteAnyComment).HasColumnName("delete_any_comment");
            entity.Property(e => e.DeleteAnyPost).HasColumnName("delete_any_post");
            entity.Property(e => e.RefactorAnyPost).HasColumnName("refactor_any_post");
            entity.Property(e => e.ValidDays).HasColumnName("valid_days");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("topic_pk");

            entity.ToTable("topic", "Forum");

            entity.HasIndex(e => e.Uid, "topic_unique").IsUnique();

            entity.HasIndex(e => e.Name, "topic_unique_name").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users", "Forum");

            entity.HasIndex(e => e.Uid, "users_unique").IsUnique();

            entity.HasIndex(e => e.Email, "users_unique_email").IsUnique();

            entity.HasIndex(e => e.Name, "users_unique_name").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AvatarImage)
                .HasColumnType("character varying")
                .HasColumnName("avatar_image");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Followers).HasColumnName("followers");
            entity.Property(e => e.HashPassword)
                .HasColumnType("character varying")
                .HasColumnName("hash_password");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasColumnType("character varying")
                .HasColumnName("role");
            entity.Property(e => e.RoleGet)
                .HasDefaultValueSql("now()")
                .HasColumnName("role_get");
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
