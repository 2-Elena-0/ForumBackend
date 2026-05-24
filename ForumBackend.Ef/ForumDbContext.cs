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

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostsImage> PostsImages { get; set; }

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
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");

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

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Posts)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_users_fk");

            entity.HasMany(d => d.Topics).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostsTopic",
                    r => r.HasOne<Topic>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("Topic")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("posts_topic_topic_fk"),
                    l => l.HasOne<Post>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("Post")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("posts_topic_posts_fk"),
                    j =>
                    {
                        j.HasKey("Post", "Topic").HasName("posts_topic_pk");
                        j.ToTable("posts_topic", "Forum");
                        j.IndexerProperty<Guid>("Post").HasColumnName("post");
                        j.IndexerProperty<Guid>("Topic").HasColumnName("topic");
                    });
        });

        modelBuilder.Entity<PostsImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_images_pk");

            entity.ToTable("posts_images", "Forum");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Image)
                .HasColumnType("character varying")
                .HasColumnName("image");
            entity.Property(e => e.Post).HasColumnName("post");

            entity.HasOne(d => d.PostNavigation).WithMany(p => p.PostsImages)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.Post)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_images_posts_fk");
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
            entity.Property(e => e.Uid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uid");

            entity.HasMany(d => d.PostFavorites).WithMany(p => p.UserFavorites)
                .UsingEntity<Dictionary<string, object>>(
                    "Favorite",
                    r => r.HasOne<Post>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("PostFavorite")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("favorites_posts_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("UserFavorite")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("favorites_users_fk"),
                    j =>
                    {
                        j.HasKey("UserFavorite", "PostFavorite").HasName("favorites_pk");
                        j.ToTable("favorites", "Forum");
                        j.IndexerProperty<Guid>("UserFavorite").HasColumnName("user_favorite");
                        j.IndexerProperty<Guid>("PostFavorite").HasColumnName("post_favorite");
                    });

            entity.HasMany(d => d.PostLikes).WithMany(p => p.UserLikes)
                .UsingEntity<Dictionary<string, object>>(
                    "Like",
                    r => r.HasOne<Post>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("PostLike")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("likes_posts_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("UserLike")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("likes_users_fk"),
                    j =>
                    {
                        j.HasKey("UserLike", "PostLike").HasName("likes_pk");
                        j.ToTable("likes", "Forum");
                        j.IndexerProperty<Guid>("UserLike").HasColumnName("user_like");
                        j.IndexerProperty<Guid>("PostLike").HasColumnName("post_like");
                    });

            entity.HasMany(d => d.Topics).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "InterestingTopic",
                    r => r.HasOne<Topic>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("Topic")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("interesting_topics_topic_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasPrincipalKey("Uid")
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("interesting_topics_users_fk"),
                    j =>
                    {
                        j.HasKey("User", "Topic").HasName("interesting_topics_pk");
                        j.ToTable("interesting_topics", "Forum");
                        j.IndexerProperty<Guid>("User").HasColumnName("user");
                        j.IndexerProperty<Guid>("Topic").HasColumnName("topic");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
