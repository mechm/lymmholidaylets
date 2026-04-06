# LymmHolidayLets Documentation Index

## 📁 Documentation Structure

### `/docs/database/` - Database Documentation
Performance audits, optimization guides, and database analysis reports.

- **DB_PERFORMANCE_AUDIT.md** - Comprehensive performance audit (latest)
- **DB_PERFORMANCE_ANALYSIS.md** - Performance analysis notes
- **DB_PERFORMANCE_REPORT.txt** - Performance report (text format)
- **DB_EXPERT_SUMMARY.md** - Database expert summary
- **DB_ANALYSIS_INDEX.md** - Analysis index
- **DB_DELIVERABLES.txt** - Database deliverables checklist
- **QUICK_REFERENCE_PERF_FIXES.md** - Quick reference for performance fixes

### `/docs/architecture/` - Architecture Documentation
System design, patterns, and architectural decisions.

*(Currently empty - future architecture docs go here)*

### `/docs/build-reports/` - Build & Test Reports
Build outputs and test execution reports.

- **build-output.txt** - Latest build output
- **test-results.txt** - Latest test results

---

## 🚀 Quick Links

### For Database Optimization
→ Start here: `/docs/database/DB_PERFORMANCE_AUDIT.md`

### For Development Setup
→ See: `README.md` (root)

### For Agent Instructions
→ See: `AGENTS.md` (root)

---

## 📝 Best Practices

**Where to save new documentation:**

| Type | Location | Examples |
|------|----------|----------|
| Database-related | `/docs/database/` | Performance audits, schema designs, migration guides |
| Architecture | `/docs/architecture/` | System diagrams, design decisions, patterns |
| Build/Test reports | `/docs/build-reports/` | CI/CD outputs, test coverage, profiling |
| Project-level instructions | Root directory | AGENTS.md, CLAUDE.md, README.md |
| Temporary work notes | Root directory | TODO.local.md |

**Naming Convention:**
- Use UPPERCASE for permanent docs in root (AGENTS.md, README.md)
- Use descriptive lowercase with hyphens for `/docs/` subdirectories (performance-audit-2026-04.md)
- Prefix related docs with category (DB_, ARCH_, etc.)

---

## 🗑️ Cleanup Policy

- Move build/test outputs to `/docs/build-reports/` after review
- Archive old performance audits with date suffix (DB_PERFORMANCE_AUDIT_2026-04.md)
- Delete temporary files after consolidation

---

**Last Updated:** 2026-04-04
