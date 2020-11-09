﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
	public class ChatSession
	{
		[Key]
		public int ChatSessionId { get; set; }
		public string ChatPrivateKey { get; set; }  // Effectively the chatroom name
		public List<ChatSessionMember> ChatMembers { get; set; }
		public List<ChatMessage> Messages { get; set; }
		[NotMapped]
		public ChatMessage LastMessage { get; set; }
	}
}
