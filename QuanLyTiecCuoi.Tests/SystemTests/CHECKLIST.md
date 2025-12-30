# System Tests - Quick Checklist ?

## ?? Pre-Test Checklist

### ? Code Fixes (DONE)
- [x] GroupId type fixed (int ? string) - 16 locations
- [x] PasswordHelper namespace fixed
- [x] Duplicate variable fixed (Scenario3)
- [x] IsHallAvailable logic fixed ? CRITICAL
- [x] Build successful (0 errors)

### ? Database Setup (TODO)
- [ ] SQL Server running
- [ ] Database `WeddingManagement` created
- [ ] Tables created (use `QuanLyTiecCuoi.sql`)
- [ ] **Parameters inserted** ? CRITICAL
  ```sql
  INSERT INTO Parameter (ParameterName, Value) VALUES
  ('PenaltyRate', 0.05),
  ('EnablePenalty', 1);
  ```
- [ ] **UserGroups inserted** ? CRITICAL
  ```sql
  INSERT INTO UserGroup (GroupId, GroupName) VALUES
  ('1', 'Customer'),
  ('2', 'Staff'),
  ('3', 'Admin');
  ```
- [ ] Test users created (at least 3)
- [ ] Halls created (at least 5)
- [ ] Shifts created (at least 2)
- [ ] Connection string verified in `app.config`

### ? Test Execution (TODO)
- [ ] Open Test Explorer (Ctrl+E, T)
- [ ] Build Solution (Ctrl+Shift+B)
- [ ] Run Scenario 1 (8 tests)
- [ ] Run Scenario 2 (10 tests)
- [ ] Run Scenario 3 (8 tests) ? Needs Parameters!
- [ ] Run Scenario 4 (10 tests)
- [ ] Review results

---

## ?? Critical Requirements

### Must Have for Tests to Run

1. **Parameter Table** (Scenario3 will fail without this!)
   ```sql
   SELECT * FROM Parameter WHERE ParameterName IN ('PenaltyRate', 'EnablePenalty');
   -- Should return 2 rows
   ```

2. **UserGroup with STRING GroupId** (Not int!)
   ```sql
   SELECT GroupId, TYPEOF(GroupId), GroupName FROM UserGroup;
   -- GroupId should be VARCHAR not INT
   ```

3. **Test Users**
   ```sql
   SELECT Username, GroupId FROM AppUser WHERE Username LIKE '%test%';
   -- Should return at least 3 rows
   ```

4. **Halls**
   ```sql
   SELECT COUNT(*) FROM Hall;
   -- Should return at least 5
   ```

---

## ?? Quick Run Commands

```bash
# All tests
dotnet test --filter "TestCategory=SystemTest"

# By scenario (run these in order!)
dotnet test --filter "TestCategory=Scenario1"
dotnet test --filter "TestCategory=Scenario2"
dotnet test --filter "TestCategory=Scenario3"
dotnet test --filter "TestCategory=Scenario4"

# Detailed output
dotnet test --filter "TestCategory=SystemTest" --logger "console;verbosity=detailed"
```

---

## ?? Common Problems

| Problem | Quick Fix |
|---------|-----------|
| "Parameter not found" | Insert PenaltyRate and EnablePenalty |
| "Test user not exist" | Insert test users with GroupId='1','2','3' |
| "No halls" | Insert at least 5 test halls |
| "Connection failed" | Check SQL Server is running |
| Tests skip/inconclusive | Missing test data - check above |

---

## ?? Expected Results

```
? Scenario 1: 8/8 passed
? Scenario 2: 10/10 passed
? Scenario 3: 8/8 passed
? Scenario 4: 10/10 passed
????????????????????????
? TOTAL: 36/36 passed
```

---

## ?? Documentation

| File | Purpose |
|------|---------|
| **TEST_EXECUTION_GUIDE.md** | Full setup guide ? READ FIRST |
| **QUICK_START.md** | Quick reference |
| **FINAL_STATUS_REPORT.md** | Complete status |
| This file | Quick checklist |

---

## ? Success Criteria

- [ ] All 36 tests RUN (not skipped)
- [ ] Pass rate ? 80% (29+ tests)
- [ ] No database errors
- [ ] No compilation errors

---

**Status:** ? Code Ready | ? Database Setup Needed | ? Tests Awaiting

**Next:** Setup database ? Run tests ? Review results

```bash
# When ready:
dotnet test --filter "TestCategory=SystemTest"
```
