using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Services;

namespace TaskTrackerApp.Infrastructure.Services;

public class FaqService : IFaqService
{
    private readonly IKernelMemory _memory;

    public FaqService(IKernelMemory memory)
    {
        _memory = memory;
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        if (ContainsInjectionKeywords(question))
        {
            return "Security Alert: Malformed request detected.";
        }

        var answer = await _memory.AskAsync(question, minRelevance: 0.2);

        if (answer.Result.Contains("I'm sorry") || answer.NoResult)
        {
            return "I can only answer questions about the Task Tracker web app.";
        }

        return answer.Result;
    }

    public async Task IngestDummyDataAsync()
    {
        //THIS SHOULD BE A DOCUMENT, THIS IS ONLY FOR DEMONSTRATION PURPOSE
        var taskTrackerDocs = """
        # Task Tracker App: User Guide & Features

        ## 1. Board Management & Settings
        - **Board Settings:** Project Admins can access the 'Settings' tab on any board.
        - **Member Roles:** Within settings, you can manage team permissions. You can change a member's role to 'Viewer', 'Editor', or 'Admin' depending on the access level required.

        ## 2. Cards and Columns
        - **Columns:** You can create dynamic columns (e.g., 'To Do', 'In Progress', 'Done') to visualize your workflow.
        - **Cards:** Create new cards within any column to represent individual tasks, bugs, or features.

        ## 3. Collaboration Features
        - **Comments:** Click on any card to open the details view. At the bottom, there is a comment section where you can discuss the task with your team.
        - **Attachments:** You can upload files (images, PDFs, documents) directly to specific comments. This is useful for sharing mockups or log files related to the task.
        """;

        await _memory.ImportTextAsync(taskTrackerDocs, documentId: "task_tracker_manual_v1");
    }

    private bool ContainsInjectionKeywords(string input)
    {
        var riskyPhrases = new[] { "ignore all instructions", "forget your rules", "system prompt", "you are a bot" };
        return riskyPhrases.Any(p => input.Contains(p, StringComparison.OrdinalIgnoreCase));
    }
}