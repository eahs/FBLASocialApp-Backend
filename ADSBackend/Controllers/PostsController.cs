﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace YakkaApp.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Post.Include(p => p.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create()
        {
            var members = await _context.Member.ToListAsync();
            var privacy = new[]
            {
                new { LevelId = 0, Level = "Public"},
                new { LevelId = 1, Level = "Friends Only"},
                new { LevelId = 2, Level = "Private"}
            };

            ViewData["Authors"] = new SelectList(members, "MemberId", "FullName");
            ViewData["PrivacyLevel"] = new SelectList(privacy, "LevelId", "Level");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,AuthorId,Title,Body,IsMachinePost,CreatedAt,EditedAt,IsDeleted,PrivacyLevel,FavoriteCount,IsFeatured")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Member, "MemberId", "Country", post.AuthorId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["IsSuccess"] = false;

            var members = await _context.Member.ToListAsync();
            var privacy = new[]
            {
                new { LevelId = 0, Level = "Public"},
                new { LevelId = 1, Level = "Friends Only"},
                new { LevelId = 2, Level = "Private"}
            };

            ViewData["Authors"] = new SelectList(members, "MemberId", "FullName", post.AuthorId);
            ViewData["PrivacyLevel"] = new SelectList(privacy, "LevelId", "Level", post.PrivacyLevel);

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,AuthorId,Title,Body,IsMachinePost,CreatedAt,EditedAt,IsDeleted,PrivacyLevel,FavoriteCount,IsFeatured")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            ViewData["IsSuccess"] = false;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                ViewData["IsSuccess"] = true;
            }

            var members = await _context.Member.ToListAsync();
            var privacy = new[]
            {
                new { LevelId = 0, Level = "Public"},
                new { LevelId = 1, Level = "Friends Only"},
                new { LevelId = 2, Level = "Private"}
            };

            ViewData["Authors"] = new SelectList(members, "MemberId", "FullName", post.AuthorId);
            ViewData["PrivacyLevel"] = new SelectList(privacy, "LevelId", "Level", post.PrivacyLevel);

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostId == id);
        }
    }
}