using System;
using System.Collections.Generic;

namespace TO.Data.Models.GameAbilitySystem.Preset
{
    /// <summary>
    /// 导入结果
    /// </summary>
    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ImportedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        
        public ImportResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}