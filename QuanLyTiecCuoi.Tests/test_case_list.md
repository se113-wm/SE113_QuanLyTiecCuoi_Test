# Test Case List - Wedding Management System

## Requirements Traceability Matrix (RTM)

Dựa trên SRS v1.8.0 - Tổng cộng 171 Business Rules  
**Phiên bản RTM:** v1.5 | **Cập nhật lần cuối:** 2025-12-03 | **Coverage:** 160/171 BRs (93.6%)

---

## 1. Authentication Use Cases

### UC 2.1.1.1: Login

#### BR1 - Displaying Rules (LoginWindow)

| No  | Req ID | Req Desc                   | TC ID      | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ---------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 1   | BR1    | Display LoginWindow screen | TC_BR1_001 | Verify LoginWindow displays correctly when application starts  |             |               |               |          |         |          |          |           |               |                     |
| 2   | BR1    | Display LoginWindow screen | TC_BR1_002 | Verify UsernameTextBox field is visible and editable           |             |               |               |          |         |          |          |           |               |                     |
| 3   | BR1    | Display LoginWindow screen | TC_BR1_003 | Verify PasswordBox field is visible, editable, and masks input |             |               |               |          |         |          |          |           |               |                     |
| 4   | BR1    | Display LoginWindow screen | TC_BR1_004 | Verify LoginButton is visible and clickable                    |             |               |               |          |         |          |          |           |               |                     |
| 5   | BR1    | Display LoginWindow screen | TC_BR1_005 | Verify LoginWindow layout matches design specifications        |             |               |               |          |         |          |          |           |               |                     |

#### BR2 - Validation Rules (Login Input)

| No  | Req ID | Req Desc                             | TC ID      | TC Desc                                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ---------- | ---------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 6   | BR2    | Validate username and password input | TC_BR2_001 | Verify error message MSG1 displays when Username is empty and Login clicked  |             |               |               |          |         |          |          |           |               |                     |
| 7   | BR2    | Validate username and password input | TC_BR2_002 | Verify error message MSG1 displays when Password is empty and Login clicked  |             |               |               |          |         |          |          |           |               |                     |
| 8   | BR2    | Validate username and password input | TC_BR2_003 | Verify error message MSG1 displays when both Username and Password are empty |             |               |               |          |         |          |          |           |               |                     |
| 9   | BR2    | Validate username and password input | TC_BR2_004 | Verify no validation error when both Username and Password are provided      |             |               |               |          |         |          |          |           |               |                     |
| 10  | BR2    | Validate username and password input | TC_BR2_005 | Verify validation triggers only after LoginButton click (not on field blur)  |             |               |               |          |         |          |          |           |               |                     |

#### BR3 - Querying Rules (Authentication)

| No  | Req ID | Req Desc                               | TC ID      | TC Desc                                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------------- | ---------- | --------------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 11  | BR3    | Query AppUser table for authentication | TC_BR3_001 | Verify successful login with valid Staff credentials redirects to Staff home page |             |               |               |          |         |          |          |           |               |                     |
| 12  | BR3    | Query AppUser table for authentication | TC_BR3_002 | Verify successful login with valid Admin credentials redirects to Admin home page |             |               |               |          |         |          |          |           |               |                     |
| 13  | BR3    | Query AppUser table for authentication | TC_BR3_003 | Verify error message MSG2 displays with incorrect username                        |             |               |               |          |         |          |          |           |               |                     |
| 14  | BR3    | Query AppUser table for authentication | TC_BR3_004 | Verify error message MSG2 displays with incorrect password                        |             |               |               |          |         |          |          |           |               |                     |
| 15  | BR3    | Query AppUser table for authentication | TC_BR3_005 | Verify error message MSG2 displays with both incorrect username and password      |             |               |               |          |         |          |          |           |               |                     |
| 16  | BR3    | Query AppUser table for authentication | TC_BR3_006 | Verify password is hashed using MD5Hash(Base64Encode(Password)) before query      |             |               |               |          |         |          |          |           |               |                     |
| 17  | BR3    | Query AppUser table for authentication | TC_BR3_007 | Verify success message MSG3 displays after successful login                       |             |               |               |          |         |          |          |           |               |                     |
| 18  | BR3    | Query AppUser table for authentication | TC_BR3_008 | Verify current user session is stored correctly via setCurrentUser(user)          |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.1.2: Logout

#### BR4 - Processing Rules (Logout)

| No  | Req ID | Req Desc                                 | TC ID      | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ---------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 19  | BR4    | Process logout request and clear session | TC_BR4_001 | Verify clicking LogoutButton triggers LogoutCommand in MainViewModel |             |               |               |          |         |          |          |           |               |                     |
| 20  | BR4    | Process logout request and clear session | TC_BR4_002 | Verify LoginWindow displays after logout                             |             |               |               |          |         |          |          |           |               |                     |
| 21  | BR4    | Process logout request and clear session | TC_BR4_003 | Verify database context is reinitialized via resetDatabaseContext()  |             |               |               |          |         |          |          |           |               |                     |
| 22  | BR4    | Process logout request and clear session | TC_BR4_004 | Verify current user session is cleared via clearCurrentUser()        |             |               |               |          |         |          |          |           |               |                     |
| 23  | BR4    | Process logout request and clear session | TC_BR4_005 | Verify MainWindow is closed after logout                             |             |               |               |          |         |          |          |           |               |                     |
| 24  | BR4    | Process logout request and clear session | TC_BR4_006 | Verify button visibility states are reset via LoadButtonVisibility() |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.1.3: Manage Profile

#### BR5 - Displaying Rules (AccountView)

| No  | Req ID | Req Desc                                   | TC ID      | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------------ | ---------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 25  | BR5    | Display AccountView with current user data | TC_BR5_001 | Verify AccountView displays when user clicks AccountCommand           |             |               |               |          |         |          |          |           |               |                     |
| 26  | BR5    | Display AccountView with current user data | TC_BR5_002 | Verify Username field displays current user's username                |             |               |               |          |         |          |          |           |               |                     |
| 27  | BR5    | Display AccountView with current user data | TC_BR5_003 | Verify FullName field displays current user's full name               |             |               |               |          |         |          |          |           |               |                     |
| 28  | BR5    | Display AccountView with current user data | TC_BR5_004 | Verify Email field displays current user's email                      |             |               |               |          |         |          |          |           |               |                     |
| 29  | BR5    | Display AccountView with current user data | TC_BR5_005 | Verify GroupName field displays current user's group name (read-only) |             |               |               |          |         |          |          |           |               |                     |

#### BR6 - Validation Rules (Profile Update)

| No  | Req ID | Req Desc                                   | TC ID      | TC Desc                                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------------ | ---------- | --------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 30  | BR6    | Validate profile update data before saving | TC_BR6_001 | Verify MSG16 displays when no changes made and Save clicked                 |             |               |               |          |         |          |          |           |               |                     |
| 31  | BR6    | Validate profile update data before saving | TC_BR6_002 | Verify MSG11 displays when Username is empty                                |             |               |               |          |         |          |          |           |               |                     |
| 32  | BR6    | Validate profile update data before saving | TC_BR6_003 | Verify MSG5 displays when Username already exists (duplicate)               |             |               |               |          |         |          |          |           |               |                     |
| 33  | BR6    | Validate profile update data before saving | TC_BR6_004 | Verify MSG13 displays when FullName is empty                                |             |               |               |          |         |          |          |           |               |                     |
| 34  | BR6    | Validate profile update data before saving | TC_BR6_005 | Verify MSG4 displays when Email format is invalid                           |             |               |               |          |         |          |          |           |               |                     |
| 35  | BR6    | Validate profile update data before saving | TC_BR6_006 | Verify valid email formats accepted (test@example.com, user.name@domain.co) |             |               |               |          |         |          |          |           |               |                     |

#### BR7 - Processing Rules (Profile Save)

| No  | Req ID | Req Desc                          | TC ID      | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ---------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 36  | BR7    | Update AppUser record in database | TC_BR7_001 | Verify AppUserDTO object created with updated values after validation |             |               |               |          |         |          |          |           |               |                     |
| 37  | BR7    | Update AppUser record in database | TC_BR7_002 | Verify Update() method in AppUserService updates database correctly   |             |               |               |          |         |          |          |           |               |                     |
| 38  | BR7    | Update AppUser record in database | TC_BR7_003 | Verify MSG6 success notification displays after successful update     |             |               |               |          |         |          |          |           |               |                     |
| 39  | BR7    | Update AppUser record in database | TC_BR7_004 | Verify current user session updated with new values after save        |             |               |               |          |         |          |          |           |               |                     |
| 40  | BR7    | Update AppUser record in database | TC_BR7_005 | Verify MSG113 error message displays when database exception occurs   |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.1.4: Change Password

#### BR8 - Displaying Rules (Change Password Dialog)

| No  | Req ID | Req Desc                       | TC ID      | TC Desc                                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ---------- | ----------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 41  | BR8    | Display Change Password dialog | TC_BR8_001 | Verify Change Password dialog displays when user clicks ChangePasswordCommand |             |               |               |          |         |          |          |           |               |                     |
| 42  | BR8    | Display Change Password dialog | TC_BR8_002 | Verify CurrentPassword field is visible and masks input                       |             |               |               |          |         |          |          |           |               |                     |
| 43  | BR8    | Display Change Password dialog | TC_BR8_003 | Verify NewPassword field is visible and masks input                           |             |               |               |          |         |          |          |           |               |                     |
| 44  | BR8    | Display Change Password dialog | TC_BR8_004 | Verify ConfirmPassword field is visible and masks input                       |             |               |               |          |         |          |          |           |               |                     |
| 45  | BR8    | Display Change Password dialog | TC_BR8_005 | Verify Save and Cancel buttons are visible                                    |             |               |               |          |         |          |          |           |               |                     |

#### BR9 - Validation Rules (Change Password)

| No  | Req ID | Req Desc                       | TC ID      | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ---------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 46  | BR9    | Validate change password input | TC_BR9_001 | Verify error displays when CurrentPassword is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 47  | BR9    | Validate change password input | TC_BR9_002 | Verify error displays when NewPassword is empty                         |             |               |               |          |         |          |          |           |               |                     |
| 48  | BR9    | Validate change password input | TC_BR9_003 | Verify error displays when ConfirmPassword is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 49  | BR9    | Validate change password input | TC_BR9_004 | Verify error displays when NewPassword and ConfirmPassword do not match |             |               |               |          |         |          |          |           |               |                     |
| 50  | BR9    | Validate change password input | TC_BR9_005 | Verify error displays when CurrentPassword is incorrect                 |             |               |               |          |         |          |          |           |               |                     |

#### BR10 - Processing Rules (Change Password Save)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 51  | BR10   | Update password in database | TC_BR10_001 | Verify new password is hashed using MD5Hash(Base64Encode()) before saving |             |               |               |          |         |          |          |           |               |                     |
| 52  | BR10   | Update password in database | TC_BR10_002 | Verify PasswordHash updated in AppUser table after successful change      |             |               |               |          |         |          |          |           |               |                     |
| 53  | BR10   | Update password in database | TC_BR10_003 | Verify success notification displays after password changed               |             |               |               |          |         |          |          |           |               |                     |
| 54  | BR10   | Update password in database | TC_BR10_004 | Verify user can login with new password after change                      |             |               |               |          |         |          |          |           |               |                     |
| 55  | BR10   | Update password in database | TC_BR10_005 | Verify error message displays when database exception occurs              |             |               |               |          |         |          |          |           |               |                     |

---

## 2. System Management Use Cases

### UC 2.1.2.1: View User Details

#### BR11 - Displaying Rules (UserView)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 56  | BR11   | Display UserView with user list | TC_BR11_001 | Verify UserView displays when Admin clicks UserCommand                |             |               |               |          |         |          |          |           |               |                     |
| 57  | BR11   | Display UserView with user list | TC_BR11_002 | Verify DataGrid shows list of users loaded from AppUser table         |             |               |               |          |         |          |          |           |               |                     |
| 58  | BR11   | Display UserView with user list | TC_BR11_003 | Verify users with same GroupId as current user are excluded from list |             |               |               |          |         |          |          |           |               |                     |
| 59  | BR11   | Display UserView with user list | TC_BR11_004 | Verify users with 'ADMIN' GroupId are excluded from list              |             |               |               |          |         |          |          |           |               |                     |
| 60  | BR11   | Display UserView with user list | TC_BR11_005 | Verify database context is reinitialized when UserView opens          |             |               |               |          |         |          |          |           |               |                     |

#### BR12 - Searching Rules (User Search)

| No  | Req ID | Req Desc                          | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 61  | BR12   | Search users by selected property | TC_BR12_001 | Verify search by Username filters users correctly                    |             |               |               |          |         |          |          |           |               |                     |
| 62  | BR12   | Search users by selected property | TC_BR12_002 | Verify search by FullName filters users correctly                    |             |               |               |          |         |          |          |           |               |                     |
| 63  | BR12   | Search users by selected property | TC_BR12_003 | Verify search by GroupId filters users correctly                     |             |               |               |          |         |          |          |           |               |                     |
| 64  | BR12   | Search users by selected property | TC_BR12_004 | Verify search by Email filters users correctly                       |             |               |               |          |         |          |          |           |               |                     |
| 65  | BR12   | Search users by selected property | TC_BR12_005 | Verify partial text search returns matching results (CONTAINS logic) |             |               |               |          |         |          |          |           |               |                     |

#### BR13 - Selection Rules (User Selection)

| No  | Req ID | Req Desc                      | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 66  | BR13   | Select user and populate form | TC_BR13_001 | Verify selecting user from DataGrid populates Username field        |             |               |               |          |         |          |          |           |               |                     |
| 67  | BR13   | Select user and populate form | TC_BR13_002 | Verify selecting user from DataGrid populates FullName field        |             |               |               |          |         |          |          |           |               |                     |
| 68  | BR13   | Select user and populate form | TC_BR13_003 | Verify selecting user from DataGrid populates Email field           |             |               |               |          |         |          |          |           |               |                     |
| 69  | BR13   | Select user and populate form | TC_BR13_004 | Verify selecting user from DataGrid sets SelectedUserType correctly |             |               |               |          |         |          |          |           |               |                     |
| 70  | BR13   | Select user and populate form | TC_BR13_005 | Verify NewPassword field is cleared when user is selected           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.2: Add New User

#### BR14 - Displaying Rules (Add User Form)

| No  | Req ID | Req Desc              | TC ID       | TC Desc                                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------- | ----------- | ------------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 71  | BR14   | Display add user form | TC_BR14_001 | Verify selecting "Add" action sets IsAdding = TRUE                       |             |               |               |          |         |          |          |           |               |                     |
| 72  | BR14   | Display add user form | TC_BR14_002 | Verify selecting "Add" action clears all form fields via Reset()         |             |               |               |          |         |          |          |           |               |                     |
| 73  | BR14   | Display add user form | TC_BR14_003 | Verify Username field is visible and editable in add mode                |             |               |               |          |         |          |          |           |               |                     |
| 74  | BR14   | Display add user form | TC_BR14_004 | Verify NewPassword field is visible and required in add mode             |             |               |               |          |         |          |          |           |               |                     |
| 75  | BR14   | Display add user form | TC_BR14_005 | Verify SelectedUserType dropdown is populated with available user groups |             |               |               |          |         |          |          |           |               |                     |

#### BR15 - Validation Rules (Add User)

| No  | Req ID | Req Desc                          | TC ID       | TC Desc                                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ----------- | --------------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 76  | BR15   | Validate user input before adding | TC_BR15_001 | Verify MSG11 displays when Username is empty                                      |             |               |               |          |         |          |          |           |               |                     |
| 77  | BR15   | Validate user input before adding | TC_BR15_002 | Verify MSG12 displays when NewPassword is empty and IsPasswordChangeEnabled=FALSE |             |               |               |          |         |          |          |           |               |                     |
| 78  | BR15   | Validate user input before adding | TC_BR15_003 | Verify MSG13 displays when FullName is empty                                      |             |               |               |          |         |          |          |           |               |                     |
| 79  | BR15   | Validate user input before adding | TC_BR15_004 | Verify MSG14 displays when SelectedUserType is NULL                               |             |               |               |          |         |          |          |           |               |                     |
| 80  | BR15   | Validate user input before adding | TC_BR15_005 | Verify MSG4 displays when Email format is invalid                                 |             |               |               |          |         |          |          |           |               |                     |
| 81  | BR15   | Validate user input before adding | TC_BR15_006 | Verify MSG5 displays when Username already exists (duplicate check)               |             |               |               |          |         |          |          |           |               |                     |

#### BR16 - Processing Rules (Add User Save)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 82  | BR16   | Create new user in database | TC_BR16_001 | Verify password is hashed using MD5Hash(Base64Encode()) before saving |             |               |               |          |         |          |          |           |               |                     |
| 83  | BR16   | Create new user in database | TC_BR16_002 | Verify new user is inserted into AppUser table correctly              |             |               |               |          |         |          |          |           |               |                     |
| 84  | BR16   | Create new user in database | TC_BR16_003 | Verify new user appears in UserList after successful creation         |             |               |               |          |         |          |          |           |               |                     |
| 85  | BR16   | Create new user in database | TC_BR16_004 | Verify form is reset after successful user creation                   |             |               |               |          |         |          |          |           |               |                     |
| 86  | BR16   | Create new user in database | TC_BR16_005 | Verify MSG15 success notification displays after user creation        |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.3: Edit User

#### BR17 - Displaying Rules (Edit User Form)

| No  | Req ID | Req Desc               | TC ID       | TC Desc                                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------- | ----------- | --------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 87  | BR17   | Display edit user form | TC_BR17_001 | Verify selecting "Edit" action sets IsEditing = TRUE                        |             |               |               |          |         |          |          |           |               |                     |
| 88  | BR17   | Display edit user form | TC_BR17_002 | Verify selecting user populates form with current user data                 |             |               |               |          |         |          |          |           |               |                     |
| 89  | BR17   | Display edit user form | TC_BR17_003 | Verify IsPasswordChangeEnabled checkbox controls password field editability |             |               |               |          |         |          |          |           |               |                     |
| 90  | BR17   | Display edit user form | TC_BR17_004 | Verify Reset() is called when switching to Edit mode                        |             |               |               |          |         |          |          |           |               |                     |
| 91  | BR17   | Display edit user form | TC_BR17_005 | Verify all editable fields are enabled for modification                     |             |               |               |          |         |          |          |           |               |                     |

#### BR18 - Validation Rules (Edit User)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | -------------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 92  | BR18   | Validate user edit input | TC_BR18_001 | Verify command returns false when SelectedItem is NULL                           |             |               |               |          |         |          |          |           |               |                     |
| 93  | BR18   | Validate user edit input | TC_BR18_002 | Verify MSG16 displays when no changes are detected                               |             |               |               |          |         |          |          |           |               |                     |
| 94  | BR18   | Validate user edit input | TC_BR18_003 | Verify MSG11 displays when Username is empty                                     |             |               |               |          |         |          |          |           |               |                     |
| 95  | BR18   | Validate user edit input | TC_BR18_004 | Verify MSG5 displays when Username is duplicate (excluding current user)         |             |               |               |          |         |          |          |           |               |                     |
| 96  | BR18   | Validate user edit input | TC_BR18_005 | Verify MSG13 displays when FullName is empty                                     |             |               |               |          |         |          |          |           |               |                     |
| 97  | BR18   | Validate user edit input | TC_BR18_006 | Verify MSG4 displays when Email format is invalid                                |             |               |               |          |         |          |          |           |               |                     |
| 98  | BR18   | Validate user edit input | TC_BR18_007 | Verify MSG12 displays when IsPasswordChangeEnabled=TRUE and NewPassword is empty |             |               |               |          |         |          |          |           |               |                     |

#### BR19 - Processing Rules (Edit User Save)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 99  | BR19   | Update user record in database | TC_BR19_001 | Verify existing password is retained when NewPassword is empty |             |               |               |          |         |          |          |           |               |                     |
| 100 | BR19   | Update user record in database | TC_BR19_002 | Verify new password is hashed when NewPassword is provided     |             |               |               |          |         |          |          |           |               |                     |
| 101 | BR19   | Update user record in database | TC_BR19_003 | Verify AppUser table is updated with new values                |             |               |               |          |         |          |          |           |               |                     |
| 102 | BR19   | Update user record in database | TC_BR19_004 | Verify UserList is updated at selected index                   |             |               |               |          |         |          |          |           |               |                     |
| 103 | BR19   | Update user record in database | TC_BR19_005 | Verify MSG17 success notification displays after update        |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.4: Delete User

#### BR20 - Displaying Rules (Delete User)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | ------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 104 | BR20   | Display delete user mode | TC_BR20_001 | Verify selecting "Delete" action sets IsDeleting = TRUE |             |               |               |          |         |          |          |           |               |                     |
| 105 | BR20   | Display delete user mode | TC_BR20_002 | Verify Reset() is called when switching to Delete mode  |             |               |               |          |         |          |          |           |               |                     |
| 106 | BR20   | Display delete user mode | TC_BR20_003 | Verify user list is displayed in DataGrid for selection |             |               |               |          |         |          |          |           |               |                     |

#### BR21 - Validation Rules (Delete User)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 107 | BR21   | Validate delete selection | TC_BR21_001 | Verify DeleteCommand returns false when SelectedItem is NULL |             |               |               |          |         |          |          |           |               |                     |
| 108 | BR21   | Validate delete selection | TC_BR21_002 | Verify delete button is disabled when no user is selected    |             |               |               |          |         |          |          |           |               |                     |
| 109 | BR21   | Validate delete selection | TC_BR21_003 | Verify delete button is enabled when user is selected        |             |               |               |          |         |          |          |           |               |                     |

#### BR22 - Confirmation Rules (Delete User)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 110 | BR22   | Display delete confirmation dialog | TC_BR22_001 | Verify confirmation dialog displays when delete is triggered |             |               |               |          |         |          |          |           |               |                     |
| 111 | BR22   | Display delete confirmation dialog | TC_BR22_002 | Verify clicking "No" cancels deletion and keeps user in list |             |               |               |          |         |          |          |           |               |                     |
| 112 | BR22   | Display delete confirmation dialog | TC_BR22_003 | Verify dialog shows user details being deleted               |             |               |               |          |         |          |          |           |               |                     |

#### BR23 - Processing Rules (Delete User)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 113 | BR23   | Delete user from database | TC_BR23_001 | Verify clicking "Yes" deletes user from AppUser table               |             |               |               |          |         |          |          |           |               |                     |
| 114 | BR23   | Delete user from database | TC_BR23_002 | Verify user is removed from UserList after deletion                 |             |               |               |          |         |          |          |           |               |                     |
| 115 | BR23   | Delete user from database | TC_BR23_003 | Verify form is reset after successful deletion                      |             |               |               |          |         |          |          |           |               |                     |
| 116 | BR23   | Delete user from database | TC_BR23_004 | Verify MSG18 success notification displays after deletion           |             |               |               |          |         |          |          |           |               |                     |
| 117 | BR23   | Delete user from database | TC_BR23_005 | Verify MSG113 error message displays when database exception occurs |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.5: View Permission Group Details

#### BR24 - Displaying Rules (PermissionView)

| No  | Req ID | Req Desc                               | TC ID       | TC Desc                                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------------- | ----------- | ------------------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 118 | BR24   | Display PermissionView with group list | TC_BR24_001 | Verify PermissionView displays when Admin clicks PermissionCommand             |             |               |               |          |         |          |          |           |               |                     |
| 119 | BR24   | Display PermissionView with group list | TC_BR24_002 | Verify DataGrid shows list of permission groups from UserGroup table           |             |               |               |          |         |          |          |           |               |                     |
| 120 | BR24   | Display PermissionView with group list | TC_BR24_003 | Verify 'Administrator' group is excluded from list                             |             |               |               |          |         |          |          |           |               |                     |
| 121 | BR24   | Display PermissionView with group list | TC_BR24_004 | Verify current user's group is excluded from list                              |             |               |               |          |         |          |          |           |               |                     |
| 122 | BR24   | Display PermissionView with group list | TC_BR24_005 | Verify PermissionStates dictionary is initialized with all function checkboxes |             |               |               |          |         |          |          |           |               |                     |

#### BR25 - Searching Rules (Permission Group Search)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | --------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 123 | BR25   | Search permission groups | TC_BR25_001 | Verify search by GroupName filters groups correctly |             |               |               |          |         |          |          |           |               |                     |
| 124 | BR25   | Search permission groups | TC_BR25_002 | Verify partial text search returns matching results |             |               |               |          |         |          |          |           |               |                     |
| 125 | BR25   | Search permission groups | TC_BR25_003 | Verify empty search text resets list to original    |             |               |               |          |         |          |          |           |               |                     |

#### BR26 - Selection Rules (Permission Group Selection)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | ---------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 126 | BR26   | Select group and display permissions | TC_BR26_001 | Verify selecting group populates GroupName field                 |             |               |               |          |         |          |          |           |               |                     |
| 127 | BR26   | Select group and display permissions | TC_BR26_002 | Verify selecting group sets IsSelected = TRUE                    |             |               |               |          |         |          |          |           |               |                     |
| 128 | BR26   | Select group and display permissions | TC_BR26_003 | Verify UpdatePermissionStates() is called to populate checkboxes |             |               |               |          |         |          |          |           |               |                     |
| 129 | BR26   | Select group and display permissions | TC_BR26_004 | Verify assigned functions have IsChecked = TRUE                  |             |               |               |          |         |          |          |           |               |                     |
| 130 | BR26   | Select group and display permissions | TC_BR26_005 | Verify non-assigned functions have IsChecked = FALSE             |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.6: Add New Permission Group

#### BR27 - Displaying Rules (Add Permission Group Form)

| No  | Req ID | Req Desc                          | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 131 | BR27   | Display add permission group form | TC_BR27_001 | Verify selecting "Add" action sets IsAdding = TRUE                        |             |               |               |          |         |          |          |           |               |                     |
| 132 | BR27   | Display add permission group form | TC_BR27_002 | Verify Reset() clears GroupName and all checkboxes                        |             |               |               |          |         |          |          |           |               |                     |
| 133 | BR27   | Display add permission group form | TC_BR27_003 | Verify all function checkboxes are displayed (Home, HallType, Hall, etc.) |             |               |               |          |         |          |          |           |               |                     |
| 134 | BR27   | Display add permission group form | TC_BR27_004 | Verify GroupName field is editable in add mode                            |             |               |               |          |         |          |          |           |               |                     |

#### BR28 - Validation Rules (Add Permission Group)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | -------------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 135 | BR28   | Validate permission group input | TC_BR28_001 | Verify MSG20 displays when GroupName is empty                                    |             |               |               |          |         |          |          |           |               |                     |
| 136 | BR28   | Validate permission group input | TC_BR28_002 | Verify MSG21 displays when GroupName equals "Administrator"                      |             |               |               |          |         |          |          |           |               |                     |
| 137 | BR28   | Validate permission group input | TC_BR28_003 | Verify MSG21 displays when GroupName contains "administrator" (case-insensitive) |             |               |               |          |         |          |          |           |               |                     |
| 138 | BR28   | Validate permission group input | TC_BR28_004 | Verify MSG21 displays when GroupName contains "admin" (case-insensitive)         |             |               |               |          |         |          |          |           |               |                     |
| 139 | BR28   | Validate permission group input | TC_BR28_005 | Verify MSG22 displays when GroupName already exists (duplicate check)            |             |               |               |          |         |          |          |           |               |                     |

#### BR29 - Processing Rules (Add Permission Group Save)

| No  | Req ID | Req Desc                                | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 140 | BR29   | Create new permission group in database | TC_BR29_001 | Verify unique GroupId is generated with format "GR" + 8 random characters |             |               |               |          |         |          |          |           |               |                     |
| 141 | BR29   | Create new permission group in database | TC_BR29_002 | Verify GroupId uniqueness is validated before creation                    |             |               |               |          |         |          |          |           |               |                     |
| 142 | BR29   | Create new permission group in database | TC_BR29_003 | Verify GroupName is trimmed before saving                                 |             |               |               |          |         |          |          |           |               |                     |
| 143 | BR29   | Create new permission group in database | TC_BR29_004 | Verify new group is inserted into UserGroup table                         |             |               |               |          |         |          |          |           |               |                     |
| 144 | BR29   | Create new permission group in database | TC_BR29_005 | Verify MSG23 success notification displays after creation                 |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.7: Edit Permission Group

#### BR30 - Displaying Rules (Edit Permission Group Form)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 145 | BR30   | Display edit permission group form | TC_BR30_001 | Verify selecting "Edit" action sets IsEditing = TRUE          |             |               |               |          |         |          |          |           |               |                     |
| 146 | BR30   | Display edit permission group form | TC_BR30_002 | Verify Reset() is called when switching to Edit mode          |             |               |               |          |         |          |          |           |               |                     |
| 147 | BR30   | Display edit permission group form | TC_BR30_003 | Verify selecting group populates form with current data       |             |               |               |          |         |          |          |           |               |                     |
| 148 | BR30   | Display edit permission group form | TC_BR30_004 | Verify UpdatePermissionStates() populates current permissions |             |               |               |          |         |          |          |           |               |                     |
| 149 | BR30   | Display edit permission group form | TC_BR30_005 | Verify all checkboxes are interactive for modification        |             |               |               |          |         |          |          |           |               |                     |

#### BR31 - Validation Rules (Edit Permission Group)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | --------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 150 | BR31   | Validate permission group edit input | TC_BR31_001 | Verify command returns false when SelectedItem is NULL                      |             |               |               |          |         |          |          |           |               |                     |
| 151 | BR31   | Validate permission group edit input | TC_BR31_002 | Verify MSG16 displays when no changes are detected                          |             |               |               |          |         |          |          |           |               |                     |
| 152 | BR31   | Validate permission group edit input | TC_BR31_003 | Verify MSG20 displays when GroupName is empty                               |             |               |               |          |         |          |          |           |               |                     |
| 153 | BR31   | Validate permission group edit input | TC_BR31_004 | Verify MSG21 displays when GroupName contains "administrator" or "admin"    |             |               |               |          |         |          |          |           |               |                     |
| 154 | BR31   | Validate permission group edit input | TC_BR31_005 | Verify MSG22 displays when GroupName is duplicate (excluding current group) |             |               |               |          |         |          |          |           |               |                     |

#### BR32 - Processing Rules (Edit Permission Group Save)

| No  | Req ID | Req Desc                            | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 155 | BR32   | Update permission group in database | TC_BR32_001 | Verify GroupName is trimmed before updating                          |             |               |               |          |         |          |          |           |               |                     |
| 156 | BR32   | Update permission group in database | TC_BR32_002 | Verify UserGroup table is updated with new GroupName                 |             |               |               |          |         |          |          |           |               |                     |
| 157 | BR32   | Update permission group in database | TC_BR32_003 | Verify PermissionState_UpdatePermission() processes checkbox changes |             |               |               |          |         |          |          |           |               |                     |
| 158 | BR32   | Update permission group in database | TC_BR32_004 | Verify Permission table is updated when checkboxes are toggled       |             |               |               |          |         |          |          |           |               |                     |
| 159 | BR32   | Update permission group in database | TC_BR32_005 | Verify MSG24 success notification displays after update              |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.8: Delete Permission Group

#### BR33 - Displaying Rules (Delete Permission Group)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 160 | BR33   | Display delete permission group mode | TC_BR33_001 | Verify selecting "Delete" action sets IsDeleting = TRUE  |             |               |               |          |         |          |          |           |               |                     |
| 161 | BR33   | Display delete permission group mode | TC_BR33_002 | Verify Reset() is called when switching to Delete mode   |             |               |               |          |         |          |          |           |               |                     |
| 162 | BR33   | Display delete permission group mode | TC_BR33_003 | Verify group list is displayed in DataGrid for selection |             |               |               |          |         |          |          |           |               |                     |

#### BR34 - Reference Check Rules (Delete Permission Group)

| No  | Req ID | Req Desc                                 | TC ID       | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ----------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 163 | BR34   | Check for referenced users before delete | TC_BR34_001 | Verify hasReferences() checks if users exist in selected group |             |               |               |          |         |          |          |           |               |                     |
| 164 | BR34   | Check for referenced users before delete | TC_BR34_002 | Verify MSG25 displays when users exist in the group            |             |               |               |          |         |          |          |           |               |                     |
| 165 | BR34   | Check for referenced users before delete | TC_BR34_003 | Verify delete is blocked when group has referenced users       |             |               |               |          |         |          |          |           |               |                     |
| 166 | BR34   | Check for referenced users before delete | TC_BR34_004 | Verify referenced user count is displayed in warning message   |             |               |               |          |         |          |          |           |               |                     |

#### BR35 - Confirmation Rules (Delete Permission Group)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 167 | BR35   | Display delete confirmation dialog | TC_BR35_001 | Verify confirmation dialog displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 168 | BR35   | Display delete confirmation dialog | TC_BR35_002 | Verify clicking "No" cancels deletion                        |             |               |               |          |         |          |          |           |               |                     |
| 169 | BR35   | Display delete confirmation dialog | TC_BR35_003 | Verify dialog closes without changes when "No" is clicked    |             |               |               |          |         |          |          |           |               |                     |

#### BR36 - Processing Rules (Delete Permission Group)

| No  | Req ID | Req Desc                              | TC ID       | TC Desc                                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------- | ----------- | -------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 170 | BR36   | Delete permission group from database | TC_BR36_001 | Verify clicking "Yes" deletes permission assignments from Permission table |             |               |               |          |         |          |          |           |               |                     |
| 171 | BR36   | Delete permission group from database | TC_BR36_002 | Verify group is deleted from UserGroup table after permissions             |             |               |               |          |         |          |          |           |               |                     |
| 172 | BR36   | Delete permission group from database | TC_BR36_003 | Verify deletion is performed in transaction                                |             |               |               |          |         |          |          |           |               |                     |
| 173 | BR36   | Delete permission group from database | TC_BR36_004 | Verify group is removed from GroupList after deletion                      |             |               |               |          |         |          |          |           |               |                     |
| 174 | BR36   | Delete permission group from database | TC_BR36_005 | Verify MSG26 success notification displays after deletion                  |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.2.9: Manage System Parameters

#### BR37 - Displaying Rules (ParameterView)

| No  | Req ID | Req Desc                                   | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------------ | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 175 | BR37   | Display ParameterView with system settings | TC_BR37_001 | Verify ParameterView displays when Admin clicks ParameterCommand   |             |               |               |          |         |          |          |           |               |                     |
| 176 | BR37   | Display ParameterView with system settings | TC_BR37_002 | Verify all parameters are loaded from Parameter table              |             |               |               |          |         |          |          |           |               |                     |
| 177 | BR37   | Display ParameterView with system settings | TC_BR37_003 | Verify EnablePenalty checkbox displays correctly (Có/Không)        |             |               |               |          |         |          |          |           |               |                     |
| 178 | BR37   | Display ParameterView with system settings | TC_BR37_004 | Verify PenaltyRate field displays current percentage value         |             |               |               |          |         |          |          |           |               |                     |
| 179 | BR37   | Display ParameterView with system settings | TC_BR37_005 | Verify MinDepositRate field displays current percentage value      |             |               |               |          |         |          |          |           |               |                     |
| 180 | BR37   | Display ParameterView with system settings | TC_BR37_006 | Verify MinReserveTableRate field displays current percentage value |             |               |               |          |         |          |          |           |               |                     |

#### BR38 - Validation Rules (System Parameters)

| No  | Req ID | Req Desc                                | TC ID       | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------- | ----------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 181 | BR38   | Validate parameter values before saving | TC_BR38_001 | Verify MSG10 displays when PenaltyRate is empty                       |             |               |               |          |         |          |          |           |               |                     |
| 182 | BR38   | Validate parameter values before saving | TC_BR38_002 | Verify MSG10 displays when MinDepositRate is empty                    |             |               |               |          |         |          |          |           |               |                     |
| 183 | BR38   | Validate parameter values before saving | TC_BR38_003 | Verify MSG10 displays when MinReserveTableRate is empty               |             |               |               |          |         |          |          |           |               |                     |
| 184 | BR38   | Validate parameter values before saving | TC_BR38_004 | Verify MSG30 displays when PenaltyRate is not numeric                 |             |               |               |          |         |          |          |           |               |                     |
| 185 | BR38   | Validate parameter values before saving | TC_BR38_005 | Verify MSG30 displays when MinDepositRate is not numeric              |             |               |               |          |         |          |          |           |               |                     |
| 186 | BR38   | Validate parameter values before saving | TC_BR38_006 | Verify MSG30 displays when MinReserveTableRate is not numeric         |             |               |               |          |         |          |          |           |               |                     |
| 187 | BR38   | Validate parameter values before saving | TC_BR38_007 | Verify MSG29 displays when PenaltyRate is out of bounds (0-1)         |             |               |               |          |         |          |          |           |               |                     |
| 188 | BR38   | Validate parameter values before saving | TC_BR38_008 | Verify MSG29 displays when MinDepositRate is out of bounds (0-1)      |             |               |               |          |         |          |          |           |               |                     |
| 189 | BR38   | Validate parameter values before saving | TC_BR38_009 | Verify MSG29 displays when MinReserveTableRate is out of bounds (0-1) |             |               |               |          |         |          |          |           |               |                     |

#### BR39 - Processing Rules (System Parameters Save)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 190 | BR39   | Update system parameters in database | TC_BR39_001 | Verify EnablePenalty parameter is updated in Parameter table       |             |               |               |          |         |          |          |           |               |                     |
| 191 | BR39   | Update system parameters in database | TC_BR39_002 | Verify PenaltyRate parameter is updated in Parameter table         |             |               |               |          |         |          |          |           |               |                     |
| 192 | BR39   | Update system parameters in database | TC_BR39_003 | Verify MinDepositRate parameter is updated in Parameter table      |             |               |               |          |         |          |          |           |               |                     |
| 193 | BR39   | Update system parameters in database | TC_BR39_004 | Verify MinReserveTableRate parameter is updated in Parameter table |             |               |               |          |         |          |          |           |               |                     |
| 194 | BR39   | Update system parameters in database | TC_BR39_005 | Verify success notification displays after all parameters updated  |             |               |               |          |         |          |          |           |               |                     |
| 195 | BR39   | Update system parameters in database | TC_BR39_006 | Verify form reloads with updated values after save                 |             |               |               |          |         |          |          |           |               |                     |

#### BR40 - Error Handling Rules (System Parameters)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 196 | BR40   | Handle parameter update errors | TC_BR40_001 | Verify transaction rollback when update fails                  |             |               |               |          |         |          |          |           |               |                     |
| 197 | BR40   | Handle parameter update errors | TC_BR40_002 | Verify MSG28 error notification displays when exception occurs |             |               |               |          |         |          |          |           |               |                     |
| 198 | BR40   | Handle parameter update errors | TC_BR40_003 | Verify parameter values remain unchanged after failed update   |             |               |               |          |         |          |          |           |               |                     |
| 199 | BR40   | Handle parameter update errors | TC_BR40_004 | Verify user can retry after error is displayed                 |             |               |               |          |         |          |          |           |               |                     |
| 200 | BR40   | Handle parameter update errors | TC_BR40_005 | Verify database connection error is handled gracefully         |             |               |               |          |         |          |          |           |               |                     |

---

## 3. Master Data Management Use Cases

### UC 2.1.3.1: View Hall Details

#### BR41 - Displaying Rules (HallView)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 201 | BR41   | Display HallView with hall list | TC_BR41_001 | Verify HallView displays when user clicks HallCommand                |             |               |               |          |         |          |          |           |               |                     |
| 202 | BR41   | Display HallView with hall list | TC_BR41_002 | Verify database context is reinitialized via resetDatabaseContext()  |             |               |               |          |         |          |          |           |               |                     |
| 203 | BR41   | Display HallView with hall list | TC_BR41_003 | Verify HallViewModel loads halls from database using GetAll() method |             |               |               |          |         |          |          |           |               |                     |
| 204 | BR41   | Display HallView with hall list | TC_BR41_004 | Verify DataGrid displays all halls with correct columns              |             |               |               |          |         |          |          |           |               |                     |
| 205 | BR41   | Display HallView with hall list | TC_BR41_005 | Verify hall type information is displayed for each hall              |             |               |               |          |         |          |          |           |               |                     |

#### BR42 - Searching Rules (Hall Search)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 206 | BR42   | Filter halls by search criteria | TC_BR42_001 | Verify PerformSearch() filters by HallName when selected      |             |               |               |          |         |          |          |           |               |                     |
| 207 | BR42   | Filter halls by search criteria | TC_BR42_002 | Verify PerformSearch() filters by HallTypeName when selected  |             |               |               |          |         |          |          |           |               |                     |
| 208 | BR42   | Filter halls by search criteria | TC_BR42_003 | Verify PerformSearch() filters by MaxTableCount when selected |             |               |               |          |         |          |          |           |               |                     |
| 209 | BR42   | Filter halls by search criteria | TC_BR42_004 | Verify search is case-insensitive                             |             |               |               |          |         |          |          |           |               |                     |
| 210 | BR42   | Filter halls by search criteria | TC_BR42_005 | Verify empty search returns all halls                         |             |               |               |          |         |          |          |           |               |                     |

#### BR43 - Selection Rules (Hall Selection)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 211 | BR43   | Select hall and display details | TC_BR43_001 | Verify setSelectedItem triggers when user selects hall from DataGrid |             |               |               |          |         |          |          |           |               |                     |
| 212 | BR43   | Select hall and display details | TC_BR43_002 | Verify HallName field is populated with selected hall name           |             |               |               |          |         |          |          |           |               |                     |
| 213 | BR43   | Select hall and display details | TC_BR43_003 | Verify SelectedHallType is set to selected hall's type               |             |               |               |          |         |          |          |           |               |                     |
| 214 | BR43   | Select hall and display details | TC_BR43_004 | Verify MaxTableCount field is populated correctly                    |             |               |               |          |         |          |          |           |               |                     |
| 215 | BR43   | Select hall and display details | TC_BR43_005 | Verify Note field is populated with selected hall note               |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.2: Add New Hall

#### BR44 - Displaying Rules (Add Hall Form)

| No  | Req ID | Req Desc              | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------- | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 216 | BR44   | Display add hall form | TC_BR44_001 | Verify IsAdding=TRUE when user selects "Thêm" action                    |             |               |               |          |         |          |          |           |               |                     |
| 217 | BR44   | Display add hall form | TC_BR44_002 | Verify IsEditing=FALSE, IsDeleting=FALSE, IsExporting=FALSE when adding |             |               |               |          |         |          |          |           |               |                     |
| 218 | BR44   | Display add hall form | TC_BR44_003 | Verify Reset() clears all form fields                                   |             |               |               |          |         |          |          |           |               |                     |
| 219 | BR44   | Display add hall form | TC_BR44_004 | Verify HallType dropdown is populated with all hall types               |             |               |               |          |         |          |          |           |               |                     |
| 220 | BR44   | Display add hall form | TC_BR44_005 | Verify AddCommand button is visible when IsAdding=TRUE                  |             |               |               |          |         |          |          |           |               |                     |

#### BR45 - Validation Rules (Add Hall)

| No  | Req ID | Req Desc                         | TC ID       | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ----------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 221 | BR45   | Validate hall data before saving | TC_BR45_001 | Verify MSG32 displays when HallName is empty                   |             |               |               |          |         |          |          |           |               |                     |
| 222 | BR45   | Validate hall data before saving | TC_BR45_002 | Verify MSG33 displays when SelectedHallType is null            |             |               |               |          |         |          |          |           |               |                     |
| 223 | BR45   | Validate hall data before saving | TC_BR45_003 | Verify MSG34 displays when MaxTableCount is empty              |             |               |               |          |         |          |          |           |               |                     |
| 224 | BR45   | Validate hall data before saving | TC_BR45_004 | Verify MSG35 displays when MaxTableCount is not numeric        |             |               |               |          |         |          |          |           |               |                     |
| 225 | BR45   | Validate hall data before saving | TC_BR45_005 | Verify MSG36 displays when MaxTableCount <= 0                  |             |               |               |          |         |          |          |           |               |                     |
| 226 | BR45   | Validate hall data before saving | TC_BR45_006 | Verify MSG37 displays when HallName already exists (duplicate) |             |               |               |          |         |          |          |           |               |                     |

#### BR46 - Processing Rules (Add Hall Save)

| No  | Req ID | Req Desc                      | TC ID       | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ----------- | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 227 | BR46   | Insert new hall into database | TC_BR46_001 | Verify HallDTO is created with trimmed HallName       |             |               |               |          |         |          |          |           |               |                     |
| 228 | BR46   | Insert new hall into database | TC_BR46_002 | Verify Create() method in HallService is called       |             |               |               |          |         |          |          |           |               |                     |
| 229 | BR46   | Insert new hall into database | TC_BR46_003 | Verify new hall is added to HallList after creation   |             |               |               |          |         |          |          |           |               |                     |
| 230 | BR46   | Insert new hall into database | TC_BR46_004 | Verify Reset() is called to clear form after save     |             |               |               |          |         |          |          |           |               |                     |
| 231 | BR46   | Insert new hall into database | TC_BR46_005 | Verify MSG38 success notification displays after save |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.3: Edit Hall

#### BR47 - Displaying Rules (Edit Hall Form)

| No  | Req ID | Req Desc                                 | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 232 | BR47   | Display edit hall form with current data | TC_BR47_001 | Verify IsEditing=TRUE when user selects "Sửa" action                    |             |               |               |          |         |          |          |           |               |                     |
| 233 | BR47   | Display edit hall form with current data | TC_BR47_002 | Verify IsAdding=FALSE, IsDeleting=FALSE, IsExporting=FALSE when editing |             |               |               |          |         |          |          |           |               |                     |
| 234 | BR47   | Display edit hall form with current data | TC_BR47_003 | Verify form is populated with selected hall data                        |             |               |               |          |         |          |          |           |               |                     |
| 235 | BR47   | Display edit hall form with current data | TC_BR47_004 | Verify EditCommand button is visible when IsEditing=TRUE                |             |               |               |          |         |          |          |           |               |                     |
| 236 | BR47   | Display edit hall form with current data | TC_BR47_005 | Verify hall selection triggers setSelectedItem to populate form         |             |               |               |          |         |          |          |           |               |                     |

#### BR48 - Validation Rules (Edit Hall)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ----------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 237 | BR48   | Validate edited hall data | TC_BR48_001 | Verify CanEdit() returns false when SelectedItem is null    |             |               |               |          |         |          |          |           |               |                     |
| 238 | BR48   | Validate edited hall data | TC_BR48_002 | Verify MSG16 displays when no changes detected              |             |               |               |          |         |          |          |           |               |                     |
| 239 | BR48   | Validate edited hall data | TC_BR48_003 | Verify MSG32 displays when HallName is empty                |             |               |               |          |         |          |          |           |               |                     |
| 240 | BR48   | Validate edited hall data | TC_BR48_004 | Verify MSG33 displays when SelectedHallType is null         |             |               |               |          |         |          |          |           |               |                     |
| 241 | BR48   | Validate edited hall data | TC_BR48_005 | Verify MSG34 displays when MaxTableCount is empty           |             |               |               |          |         |          |          |           |               |                     |
| 242 | BR48   | Validate edited hall data | TC_BR48_006 | Verify MSG37 displays when HallName duplicates another hall |             |               |               |          |         |          |          |           |               |                     |

#### BR49 - Processing Rules (Edit Hall Save)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | ------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 243 | BR49   | Update hall record in database | TC_BR49_001 | Verify HallDTO is created with updated values           |             |               |               |          |         |          |          |           |               |                     |
| 244 | BR49   | Update hall record in database | TC_BR49_002 | Verify Update() method in HallService is called         |             |               |               |          |         |          |          |           |               |                     |
| 245 | BR49   | Update hall record in database | TC_BR49_003 | Verify HallList is updated at selected index            |             |               |               |          |         |          |          |           |               |                     |
| 246 | BR49   | Update hall record in database | TC_BR49_004 | Verify Reset() is called after update                   |             |               |               |          |         |          |          |           |               |                     |
| 247 | BR49   | Update hall record in database | TC_BR49_005 | Verify MSG39 success notification displays after update |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.4: Delete Hall

#### BR50 - Displaying Rules (Delete Hall)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 248 | BR50   | Display delete hall mode | TC_BR50_001 | Verify IsDeleting=TRUE when user selects "Xóa" action                   |             |               |               |          |         |          |          |           |               |                     |
| 249 | BR50   | Display delete hall mode | TC_BR50_002 | Verify IsAdding=FALSE, IsEditing=FALSE, IsExporting=FALSE when deleting |             |               |               |          |         |          |          |           |               |                     |
| 250 | BR50   | Display delete hall mode | TC_BR50_003 | Verify Reset() is called when entering delete mode                      |             |               |               |          |         |          |          |           |               |                     |
| 251 | BR50   | Display delete hall mode | TC_BR50_004 | Verify DataGrid displays halls for selection                            |             |               |               |          |         |          |          |           |               |                     |

#### BR51 - Reference Check Rules (Delete Hall)

| No  | Req ID | Req Desc                            | TC ID       | TC Desc                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ----------- | --------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 252 | BR51   | Check hall references before delete | TC_BR51_001 | Verify CanDelete() checks if hall exists in Booking table |             |               |               |          |         |          |          |           |               |                     |
| 253 | BR51   | Check hall references before delete | TC_BR51_002 | Verify MSG40 displays when hall has existing bookings     |             |               |               |          |         |          |          |           |               |                     |
| 254 | BR51   | Check hall references before delete | TC_BR51_003 | Verify CanDelete() returns false when hall is referenced  |             |               |               |          |         |          |          |           |               |                     |
| 255 | BR51   | Check hall references before delete | TC_BR51_004 | Verify user is informed about referenced bookings         |             |               |               |          |         |          |          |           |               |                     |

#### BR52 - Confirmation Rules (Delete Hall)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 256 | BR52   | Display delete confirmation dialog | TC_BR52_001 | Verify confirmation dialog MSG41 displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 257 | BR52   | Display delete confirmation dialog | TC_BR52_002 | Verify clicking "No" cancels deletion                              |             |               |               |          |         |          |          |           |               |                     |
| 258 | BR52   | Display delete confirmation dialog | TC_BR52_003 | Verify dialog closes without changes when "No" is clicked          |             |               |               |          |         |          |          |           |               |                     |

#### BR53 - Processing Rules (Delete Hall)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 259 | BR53   | Delete hall from database | TC_BR53_001 | Verify clicking "Yes" calls Delete() method in HallService |             |               |               |          |         |          |          |           |               |                     |
| 260 | BR53   | Delete hall from database | TC_BR53_002 | Verify hall is removed from HallList after deletion        |             |               |               |          |         |          |          |           |               |                     |
| 261 | BR53   | Delete hall from database | TC_BR53_003 | Verify hall is removed from OriginalList after deletion    |             |               |               |          |         |          |          |           |               |                     |
| 262 | BR53   | Delete hall from database | TC_BR53_004 | Verify Reset() is called to clear selection after delete   |             |               |               |          |         |          |          |           |               |                     |
| 263 | BR53   | Delete hall from database | TC_BR53_005 | Verify MSG42 success notification displays after deletion  |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.5: Export Halls to Excel

#### BR54 - Displaying Rules (Export Halls)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 264 | BR54   | Display export halls mode | TC_BR54_001 | Verify IsExporting=TRUE when user selects "Xuất Excel" action           |             |               |               |          |         |          |          |           |               |                     |
| 265 | BR54   | Display export halls mode | TC_BR54_002 | Verify IsAdding=FALSE, IsEditing=FALSE, IsDeleting=FALSE when exporting |             |               |               |          |         |          |          |           |               |                     |
| 266 | BR54   | Display export halls mode | TC_BR54_003 | Verify user can apply filter criteria before export                     |             |               |               |          |         |          |          |           |               |                     |
| 267 | BR54   | Display export halls mode | TC_BR54_004 | Verify ExportToExcelCommand button is visible when IsExporting=TRUE     |             |               |               |          |         |          |          |           |               |                     |

#### BR55 - Validation Rules (Export Halls)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | -------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 268 | BR55   | Validate data before export | TC_BR55_001 | Verify ExportToExcel() checks if HallList has data |             |               |               |          |         |          |          |           |               |                     |
| 269 | BR55   | Validate data before export | TC_BR55_002 | Verify MSG19 displays when HallList is NULL        |             |               |               |          |         |          |          |           |               |                     |
| 270 | BR55   | Validate data before export | TC_BR55_003 | Verify MSG19 displays when HallList.Count = 0      |             |               |               |          |         |          |          |           |               |                     |
| 271 | BR55   | Validate data before export | TC_BR55_004 | Verify export proceeds when HallList has data      |             |               |               |          |         |          |          |           |               |                     |

#### BR56 - Processing Rules (Export Halls)

| No  | Req ID | Req Desc                      | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 272 | BR56   | Generate Excel file for halls | TC_BR56_001 | Verify XLWorkbook is created using ClosedXML library                |             |               |               |          |         |          |          |           |               |                     |
| 273 | BR56   | Generate Excel file for halls | TC_BR56_002 | Verify worksheet "Danh sách Sảnh" is created                        |             |               |               |          |         |          |          |           |               |                     |
| 274 | BR56   | Generate Excel file for halls | TC_BR56_003 | Verify columns include: Tên sảnh, Loại sảnh, Số bàn tối đa, Ghi chú |             |               |               |          |         |          |          |           |               |                     |
| 275 | BR56   | Generate Excel file for halls | TC_BR56_004 | Verify header formatting: bold, light gray background, centered     |             |               |               |          |         |          |          |           |               |                     |
| 276 | BR56   | Generate Excel file for halls | TC_BR56_005 | Verify filename format: "DanhSachSanh\_[yyyyMMddHHmmss].xlsx"       |             |               |               |          |         |          |          |           |               |                     |
| 277 | BR56   | Generate Excel file for halls | TC_BR56_006 | Verify SaveFileDialog opens for user to choose location             |             |               |               |          |         |          |          |           |               |                     |
| 278 | BR56   | Generate Excel file for halls | TC_BR56_007 | Verify MSG43 success notification displays after export             |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.6: View Hall Type Details

#### BR57 - Displaying Rules (HallTypeView)

| No  | Req ID | Req Desc                                 | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 279 | BR57   | Display HallTypeView with hall type list | TC_BR57_001 | Verify HallTypeView displays when user clicks HallTypeCommand       |             |               |               |          |         |          |          |           |               |                     |
| 280 | BR57   | Display HallTypeView with hall type list | TC_BR57_002 | Verify database context is reinitialized via resetDatabaseContext() |             |               |               |          |         |          |          |           |               |                     |
| 281 | BR57   | Display HallTypeView with hall type list | TC_BR57_003 | Verify HallTypeViewModel loads hall types using GetAll() method     |             |               |               |          |         |          |          |           |               |                     |
| 282 | BR57   | Display HallTypeView with hall type list | TC_BR57_004 | Verify DataGrid displays all hall types with correct columns        |             |               |               |          |         |          |          |           |               |                     |
| 283 | BR57   | Display HallTypeView with hall type list | TC_BR57_005 | Verify MinTablePrice is displayed with proper currency format       |             |               |               |          |         |          |          |           |               |                     |

#### BR58 - Searching Rules (Hall Type Search)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 284 | BR58   | Filter hall types by search criteria | TC_BR58_001 | Verify PerformSearch() filters by HallTypeName when selected  |             |               |               |          |         |          |          |           |               |                     |
| 285 | BR58   | Filter hall types by search criteria | TC_BR58_002 | Verify PerformSearch() filters by MinTablePrice when selected |             |               |               |          |         |          |          |           |               |                     |
| 286 | BR58   | Filter hall types by search criteria | TC_BR58_003 | Verify search is case-insensitive for HallTypeName            |             |               |               |          |         |          |          |           |               |                     |
| 287 | BR58   | Filter hall types by search criteria | TC_BR58_004 | Verify empty search returns all hall types                    |             |               |               |          |         |          |          |           |               |                     |

#### BR59 - Selection Rules (Hall Type Selection)

| No  | Req ID | Req Desc                             | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 288 | BR59   | Select hall type and display details | TC_BR59_001 | Verify setSelectedItem triggers when user selects hall type from DataGrid |             |               |               |          |         |          |          |           |               |                     |
| 289 | BR59   | Select hall type and display details | TC_BR59_002 | Verify HallTypeName field is populated with selected hall type name       |             |               |               |          |         |          |          |           |               |                     |
| 290 | BR59   | Select hall type and display details | TC_BR59_003 | Verify MinTablePrice field is populated correctly                         |             |               |               |          |         |          |          |           |               |                     |
| 291 | BR59   | Select hall type and display details | TC_BR59_004 | Verify user can proceed to edit or delete from selection                  |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.7: Add New Hall Type

#### BR60 - Displaying Rules (Add Hall Type Form)

| No  | Req ID | Req Desc                   | TC ID       | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ----------- | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 292 | BR60   | Display add hall type form | TC_BR60_001 | Verify IsAdding=TRUE when user selects "Thêm" action       |             |               |               |          |         |          |          |           |               |                     |
| 293 | BR60   | Display add hall type form | TC_BR60_002 | Verify Reset() clears all form fields                      |             |               |               |          |         |          |          |           |               |                     |
| 294 | BR60   | Display add hall type form | TC_BR60_003 | Verify form displays HallTypeName and MinTablePrice fields |             |               |               |          |         |          |          |           |               |                     |
| 295 | BR60   | Display add hall type form | TC_BR60_004 | Verify AddCommand button is visible when IsAdding=TRUE     |             |               |               |          |         |          |          |           |               |                     |

#### BR61 - Validation Rules (Add Hall Type)

| No  | Req ID | Req Desc                              | TC ID       | TC Desc                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------- | ----------- | ------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 296 | BR61   | Validate hall type data before saving | TC_BR61_001 | Verify MSG44 displays when HallTypeName is empty        |             |               |               |          |         |          |          |           |               |                     |
| 297 | BR61   | Validate hall type data before saving | TC_BR61_002 | Verify MSG45 displays when MinTablePrice is empty       |             |               |               |          |         |          |          |           |               |                     |
| 298 | BR61   | Validate hall type data before saving | TC_BR61_003 | Verify MSG46 displays when MinTablePrice is not numeric |             |               |               |          |         |          |          |           |               |                     |
| 299 | BR61   | Validate hall type data before saving | TC_BR61_004 | Verify MSG47 displays when MinTablePrice <= 0           |             |               |               |          |         |          |          |           |               |                     |
| 300 | BR61   | Validate hall type data before saving | TC_BR61_005 | Verify MSG48 displays when HallTypeName already exists  |             |               |               |          |         |          |          |           |               |                     |

#### BR62 - Processing Rules (Add Hall Type Save)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 301 | BR62   | Insert new hall type into database | TC_BR62_001 | Verify HallTypeDTO is created with trimmed HallTypeName      |             |               |               |          |         |          |          |           |               |                     |
| 302 | BR62   | Insert new hall type into database | TC_BR62_002 | Verify MinTablePrice is parsed as decimal                    |             |               |               |          |         |          |          |           |               |                     |
| 303 | BR62   | Insert new hall type into database | TC_BR62_003 | Verify Create() method in HallTypeService is called          |             |               |               |          |         |          |          |           |               |                     |
| 304 | BR62   | Insert new hall type into database | TC_BR62_004 | Verify new hall type is added to HallTypeList after creation |             |               |               |          |         |          |          |           |               |                     |
| 305 | BR62   | Insert new hall type into database | TC_BR62_005 | Verify MSG49 success notification displays after save        |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.8: Edit Hall Type

#### BR63 - Displaying Rules (Edit Hall Type Form)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 306 | BR63   | Display edit hall type form | TC_BR63_001 | Verify IsEditing=TRUE when user selects "Sửa" action     |             |               |               |          |         |          |          |           |               |                     |
| 307 | BR63   | Display edit hall type form | TC_BR63_002 | Verify form is populated with selected hall type data    |             |               |               |          |         |          |          |           |               |                     |
| 308 | BR63   | Display edit hall type form | TC_BR63_003 | Verify EditCommand button is visible when IsEditing=TRUE |             |               |               |          |         |          |          |           |               |                     |
| 309 | BR63   | Display edit hall type form | TC_BR63_004 | Verify hall type selection triggers setSelectedItem      |             |               |               |          |         |          |          |           |               |                     |

#### BR64 - Validation Rules (Edit Hall Type)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 310 | BR64   | Validate edited hall type data | TC_BR64_001 | Verify CanEdit() returns false when SelectedItem is null             |             |               |               |          |         |          |          |           |               |                     |
| 311 | BR64   | Validate edited hall type data | TC_BR64_002 | Verify MSG16 displays when no changes detected                       |             |               |               |          |         |          |          |           |               |                     |
| 312 | BR64   | Validate edited hall type data | TC_BR64_003 | Verify MSG44 displays when HallTypeName is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 313 | BR64   | Validate edited hall type data | TC_BR64_004 | Verify MSG45 displays when MinTablePrice is empty                    |             |               |               |          |         |          |          |           |               |                     |
| 314 | BR64   | Validate edited hall type data | TC_BR64_005 | Verify MSG48 displays when HallTypeName duplicates another hall type |             |               |               |          |         |          |          |           |               |                     |

#### BR65 - Processing Rules (Edit Hall Type Save)

| No  | Req ID | Req Desc                            | TC ID       | TC Desc                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ----------- | ------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 315 | BR65   | Update hall type record in database | TC_BR65_001 | Verify HallTypeDTO is created with updated values       |             |               |               |          |         |          |          |           |               |                     |
| 316 | BR65   | Update hall type record in database | TC_BR65_002 | Verify Update() method in HallTypeService is called     |             |               |               |          |         |          |          |           |               |                     |
| 317 | BR65   | Update hall type record in database | TC_BR65_003 | Verify HallTypeList is updated at selected index        |             |               |               |          |         |          |          |           |               |                     |
| 318 | BR65   | Update hall type record in database | TC_BR65_004 | Verify Reset() is called after update                   |             |               |               |          |         |          |          |           |               |                     |
| 319 | BR65   | Update hall type record in database | TC_BR65_005 | Verify MSG50 success notification displays after update |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.9: Delete Hall Type

#### BR66 - Displaying Rules (Delete Hall Type)

| No  | Req ID | Req Desc                      | TC ID       | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ----------- | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 320 | BR66   | Display delete hall type mode | TC_BR66_001 | Verify IsDeleting=TRUE when user selects "Xóa" action |             |               |               |          |         |          |          |           |               |                     |
| 321 | BR66   | Display delete hall type mode | TC_BR66_002 | Verify Reset() is called when entering delete mode    |             |               |               |          |         |          |          |           |               |                     |
| 322 | BR66   | Display delete hall type mode | TC_BR66_003 | Verify DataGrid displays hall types for selection     |             |               |               |          |         |          |          |           |               |                     |

#### BR67 - Reference Check Rules (Delete Hall Type)

| No  | Req ID | Req Desc                                 | TC ID       | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ----------- | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 323 | BR67   | Check hall type references before delete | TC_BR67_001 | Verify CanDelete() checks if hall type exists in Hall table   |             |               |               |          |         |          |          |           |               |                     |
| 324 | BR67   | Check hall type references before delete | TC_BR67_002 | Verify MSG51 displays when hall type has existing halls       |             |               |               |          |         |          |          |           |               |                     |
| 325 | BR67   | Check hall type references before delete | TC_BR67_003 | Verify CanDelete() returns false when hall type is referenced |             |               |               |          |         |          |          |           |               |                     |
| 326 | BR67   | Check hall type references before delete | TC_BR67_004 | Verify user is informed about referenced halls                |             |               |               |          |         |          |          |           |               |                     |

#### BR68 - Confirmation Rules (Delete Hall Type)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 327 | BR68   | Display delete confirmation dialog | TC_BR68_001 | Verify confirmation dialog displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 328 | BR68   | Display delete confirmation dialog | TC_BR68_002 | Verify clicking "No" cancels deletion                        |             |               |               |          |         |          |          |           |               |                     |
| 329 | BR68   | Display delete confirmation dialog | TC_BR68_003 | Verify dialog closes without changes when "No" is clicked    |             |               |               |          |         |          |          |           |               |                     |

#### BR69 - Processing Rules (Delete Hall Type)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 330 | BR69   | Delete hall type from database | TC_BR69_001 | Verify clicking "Yes" calls Delete() method in HallTypeService |             |               |               |          |         |          |          |           |               |                     |
| 331 | BR69   | Delete hall type from database | TC_BR69_002 | Verify hall type is removed from HallTypeList after deletion   |             |               |               |          |         |          |          |           |               |                     |
| 332 | BR69   | Delete hall type from database | TC_BR69_003 | Verify hall type is removed from OriginalList after deletion   |             |               |               |          |         |          |          |           |               |                     |
| 333 | BR69   | Delete hall type from database | TC_BR69_004 | Verify Reset() is called to clear selection after delete       |             |               |               |          |         |          |          |           |               |                     |
| 334 | BR69   | Delete hall type from database | TC_BR69_005 | Verify MSG52 success notification displays after deletion      |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.10: Export Hall Types to Excel

#### BR70 - Displaying Rules (Export Hall Types)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 335 | BR70   | Display export hall types mode | TC_BR70_001 | Verify IsExporting=TRUE when user selects "Xuất Excel" action           |             |               |               |          |         |          |          |           |               |                     |
| 336 | BR70   | Display export hall types mode | TC_BR70_002 | Verify IsAdding=FALSE, IsEditing=FALSE, IsDeleting=FALSE when exporting |             |               |               |          |         |          |          |           |               |                     |
| 337 | BR70   | Display export hall types mode | TC_BR70_003 | Verify user can apply filter criteria before export                     |             |               |               |          |         |          |          |           |               |                     |
| 338 | BR70   | Display export hall types mode | TC_BR70_004 | Verify ExportToExcelCommand button is visible when IsExporting=TRUE     |             |               |               |          |         |          |          |           |               |                     |

#### BR71 - Validation Rules (Export Hall Types)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | ----------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 339 | BR71   | Validate data before export | TC_BR71_001 | Verify ExportToExcelCommand checks if HallTypeList has data |             |               |               |          |         |          |          |           |               |                     |
| 340 | BR71   | Validate data before export | TC_BR71_002 | Verify MSG19 displays when HallTypeList is NULL             |             |               |               |          |         |          |          |           |               |                     |
| 341 | BR71   | Validate data before export | TC_BR71_003 | Verify MSG19 displays when HallTypeList.Count = 0           |             |               |               |          |         |          |          |           |               |                     |
| 342 | BR71   | Validate data before export | TC_BR71_004 | Verify export proceeds when HallTypeList has data           |             |               |               |          |         |          |          |           |               |                     |

#### BR72 - Processing Rules (Export Hall Types)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ----------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 343 | BR72   | Generate Excel file for hall types | TC_BR72_001 | Verify XLWorkbook is created using ClosedXML library              |             |               |               |          |         |          |          |           |               |                     |
| 344 | BR72   | Generate Excel file for hall types | TC_BR72_002 | Verify worksheet "Danh sách loại sảnh" is created                 |             |               |               |          |         |          |          |           |               |                     |
| 345 | BR72   | Generate Excel file for hall types | TC_BR72_003 | Verify columns: "Tên loại sảnh", "Đơn giá bàn tối thiểu"          |             |               |               |          |         |          |          |           |               |                     |
| 346 | BR72   | Generate Excel file for hall types | TC_BR72_004 | Verify header formatting: bold, light gray background, centered   |             |               |               |          |         |          |          |           |               |                     |
| 347 | BR72   | Generate Excel file for hall types | TC_BR72_005 | Verify number format "#,##0" for price column                     |             |               |               |          |         |          |          |           |               |                     |
| 348 | BR72   | Generate Excel file for hall types | TC_BR72_006 | Verify filename format: "DanhSachLoaiSanh\_[yyyyMMddHHmmss].xlsx" |             |               |               |          |         |          |          |           |               |                     |
| 349 | BR72   | Generate Excel file for hall types | TC_BR72_007 | Verify MSG50 success notification displays after export           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.11: View Dish Details

#### BR73 - Displaying Rules (FoodView)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 350 | BR73   | Display FoodView with dish list | TC_BR73_001 | Verify FoodView displays when user clicks DishCommand               |             |               |               |          |         |          |          |           |               |                     |
| 351 | BR73   | Display FoodView with dish list | TC_BR73_002 | Verify database context is reinitialized via resetDatabaseContext() |             |               |               |          |         |          |          |           |               |                     |
| 352 | BR73   | Display FoodView with dish list | TC_BR73_003 | Verify FoodViewModel loads dishes using GetAll() from DishService   |             |               |               |          |         |          |          |           |               |                     |
| 353 | BR73   | Display FoodView with dish list | TC_BR73_004 | Verify DataGrid displays all dishes with correct columns            |             |               |               |          |         |          |          |           |               |                     |
| 354 | BR73   | Display FoodView with dish list | TC_BR73_005 | Verify UnitPrice is displayed with proper currency format           |             |               |               |          |         |          |          |           |               |                     |

#### BR74 - Searching Rules (Dish Search)

| No  | Req ID | Req Desc                         | TC ID       | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ----------- | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 355 | BR74   | Filter dishes by search criteria | TC_BR74_001 | Verify PerformSearch() filters by DishName when "Tên món ăn" selected |             |               |               |          |         |          |          |           |               |                     |
| 356 | BR74   | Filter dishes by search criteria | TC_BR74_002 | Verify PerformSearch() filters by UnitPrice when "Đơn giá" selected   |             |               |               |          |         |          |          |           |               |                     |
| 357 | BR74   | Filter dishes by search criteria | TC_BR74_003 | Verify PerformSearch() filters by Note when "Ghi chú" selected        |             |               |               |          |         |          |          |           |               |                     |
| 358 | BR74   | Filter dishes by search criteria | TC_BR74_004 | Verify search is case-insensitive                                     |             |               |               |          |         |          |          |           |               |                     |
| 359 | BR74   | Filter dishes by search criteria | TC_BR74_005 | Verify empty search returns all dishes                                |             |               |               |          |         |          |          |           |               |                     |

#### BR75 - Selection Rules (Dish Selection)

| No  | Req ID | Req Desc                        | TC ID       | TC Desc                                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ----------- | -------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 360 | BR75   | Select dish and display details | TC_BR75_001 | Verify setSelectedItem triggers when user selects dish from DataGrid |             |               |               |          |         |          |          |           |               |                     |
| 361 | BR75   | Select dish and display details | TC_BR75_002 | Verify DishName field is populated with selected dish name           |             |               |               |          |         |          |          |           |               |                     |
| 362 | BR75   | Select dish and display details | TC_BR75_003 | Verify UnitPrice field is populated correctly                        |             |               |               |          |         |          |          |           |               |                     |
| 363 | BR75   | Select dish and display details | TC_BR75_004 | Verify Note field is populated with selected dish note               |             |               |               |          |         |          |          |           |               |                     |
| 364 | BR75   | Select dish and display details | TC_BR75_005 | Verify RenderImageAsync() displays dish image                        |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.12: Add New Dish

#### BR76 - Displaying Rules (Add Dish Form)

| No  | Req ID | Req Desc              | TC ID       | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------- | ----------- | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 365 | BR76   | Display add dish form | TC_BR76_001 | Verify IsAdding=TRUE when user selects "Thêm" action            |             |               |               |          |         |          |          |           |               |                     |
| 366 | BR76   | Display add dish form | TC_BR76_002 | Verify Reset() clears all form fields and Image=NULL            |             |               |               |          |         |          |          |           |               |                     |
| 367 | BR76   | Display add dish form | TC_BR76_003 | Verify form displays DishName, UnitPrice, Note, image selection |             |               |               |          |         |          |          |           |               |                     |
| 368 | BR76   | Display add dish form | TC_BR76_004 | Verify AddCommand button is visible when IsAdding=TRUE          |             |               |               |          |         |          |          |           |               |                     |

#### BR77 - Validation Rules (Add Dish)

| No  | Req ID | Req Desc                         | TC ID       | TC Desc                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ----------- | ---------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 369 | BR77   | Validate dish data before saving | TC_BR77_001 | Verify MSG51 displays when OriginalList.Count >= 100 (max limit) |             |               |               |          |         |          |          |           |               |                     |
| 370 | BR77   | Validate dish data before saving | TC_BR77_002 | Verify MSG52 displays when DishName is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 371 | BR77   | Validate dish data before saving | TC_BR77_003 | Verify MSG53 displays when UnitPrice is not numeric              |             |               |               |          |         |          |          |           |               |                     |
| 372 | BR77   | Validate dish data before saving | TC_BR77_004 | Verify MSG53 displays when UnitPrice <= 0                        |             |               |               |          |         |          |          |           |               |                     |
| 373 | BR77   | Validate dish data before saving | TC_BR77_005 | Verify MSG54 displays when Note.Length > 100                     |             |               |               |          |         |          |          |           |               |                     |
| 374 | BR77   | Validate dish data before saving | TC_BR77_006 | Verify MSG55 displays when DishName already exists (duplicate)   |             |               |               |          |         |          |          |           |               |                     |

#### BR78 - Processing Rules (Add Dish Save)

| No  | Req ID | Req Desc                      | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 375 | BR78   | Insert new dish into database | TC_BR78_001 | Verify DishDTO is created with trimmed DishName                           |             |               |               |          |         |          |          |           |               |                     |
| 376 | BR78   | Insert new dish into database | TC_BR78_002 | Verify UnitPrice is parsed as decimal                                     |             |               |               |          |         |          |          |           |               |                     |
| 377 | BR78   | Insert new dish into database | TC_BR78_003 | Verify Create() method in DishService is called                           |             |               |               |          |         |          |          |           |               |                     |
| 378 | BR78   | Insert new dish into database | TC_BR78_004 | Verify image cache copied from "Food/Addcache.jpg" to "Food/[DishId].jpg" |             |               |               |          |         |          |          |           |               |                     |
| 379 | BR78   | Insert new dish into database | TC_BR78_005 | Verify new dish is added to DishList after creation                       |             |               |               |          |         |          |          |           |               |                     |
| 380 | BR78   | Insert new dish into database | TC_BR78_006 | Verify MSG56 success notification displays after save                     |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.13: Edit Dish

#### BR79 - Displaying Rules (Edit Dish Form)

| No  | Req ID | Req Desc                                 | TC ID       | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ----------- | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 381 | BR79   | Display edit dish form with current data | TC_BR79_001 | Verify IsEditing=TRUE when user selects "Sửa" action     |             |               |               |          |         |          |          |           |               |                     |
| 382 | BR79   | Display edit dish form with current data | TC_BR79_002 | Verify form is populated with selected dish data         |             |               |               |          |         |          |          |           |               |                     |
| 383 | BR79   | Display edit dish form with current data | TC_BR79_003 | Verify RenderImageAsync() displays current dish image    |             |               |               |          |         |          |          |           |               |                     |
| 384 | BR79   | Display edit dish form with current data | TC_BR79_004 | Verify EditCommand button is visible when IsEditing=TRUE |             |               |               |          |         |          |          |           |               |                     |

#### BR80 - Validation Rules (Edit Dish)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | --------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 385 | BR80   | Validate edited dish data | TC_BR80_001 | Verify CanEdit() returns false when SelectedItem is null                    |             |               |               |          |         |          |          |           |               |                     |
| 386 | BR80   | Validate edited dish data | TC_BR80_002 | Verify MSG16 displays when no changes detected (fields AND image unchanged) |             |               |               |          |         |          |          |           |               |                     |
| 387 | BR80   | Validate edited dish data | TC_BR80_003 | Verify MSG52 displays when DishName is empty                                |             |               |               |          |         |          |          |           |               |                     |
| 388 | BR80   | Validate edited dish data | TC_BR80_004 | Verify MSG53 displays when UnitPrice is not numeric or <= 0                 |             |               |               |          |         |          |          |           |               |                     |
| 389 | BR80   | Validate edited dish data | TC_BR80_005 | Verify MSG54 displays when Note.Length > 100                                |             |               |               |          |         |          |          |           |               |                     |
| 390 | BR80   | Validate edited dish data | TC_BR80_006 | Verify MSG55 displays when DishName duplicates another dish                 |             |               |               |          |         |          |          |           |               |                     |

#### BR81 - Processing Rules (Edit Dish Save)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | -------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 391 | BR81   | Update dish record in database | TC_BR81_001 | Verify existing image deleted when Image=NULL AND image file exists        |             |               |               |          |         |          |          |           |               |                     |
| 392 | BR81   | Update dish record in database | TC_BR81_002 | Verify DishDTO is created with updated values                              |             |               |               |          |         |          |          |           |               |                     |
| 393 | BR81   | Update dish record in database | TC_BR81_003 | Verify Update() method in DishService is called                            |             |               |               |          |         |          |          |           |               |                     |
| 394 | BR81   | Update dish record in database | TC_BR81_004 | Verify image cache copied from "Food/Editcache.jpg" to "Food/[DishId].jpg" |             |               |               |          |         |          |          |           |               |                     |
| 395 | BR81   | Update dish record in database | TC_BR81_005 | Verify DishList is updated at selected index                               |             |               |               |          |         |          |          |           |               |                     |
| 396 | BR81   | Update dish record in database | TC_BR81_006 | Verify MSG57 success notification displays after update                    |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.14: Delete Dish

#### BR82 - Displaying Rules (Delete Dish)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 397 | BR82   | Display delete dish mode | TC_BR82_001 | Verify IsDeleting=TRUE when user selects "Xóa" action |             |               |               |          |         |          |          |           |               |                     |
| 398 | BR82   | Display delete dish mode | TC_BR82_002 | Verify Reset() is called when entering delete mode    |             |               |               |          |         |          |          |           |               |                     |
| 399 | BR82   | Display delete dish mode | TC_BR82_003 | Verify DataGrid displays dishes for selection         |             |               |               |          |         |          |          |           |               |                     |
| 400 | BR82   | Display delete dish mode | TC_BR82_004 | Verify user can select dish to delete                 |             |               |               |          |         |          |          |           |               |                     |

#### BR83 - Reference Check Rules (Delete Dish)

| No  | Req ID | Req Desc                            | TC ID       | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ----------- | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 401 | BR83   | Check dish references before delete | TC_BR83_001 | Verify CanDelete() checks if dish exists in Menu table   |             |               |               |          |         |          |          |           |               |                     |
| 402 | BR83   | Check dish references before delete | TC_BR83_002 | Verify MSG58 displays when dish is used in bookings      |             |               |               |          |         |          |          |           |               |                     |
| 403 | BR83   | Check dish references before delete | TC_BR83_003 | Verify CanDelete() returns false when dish is referenced |             |               |               |          |         |          |          |           |               |                     |
| 404 | BR83   | Check dish references before delete | TC_BR83_004 | Verify user is informed about referenced menus           |             |               |               |          |         |          |          |           |               |                     |

#### BR84 - Confirmation Rules (Delete Dish)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 405 | BR84   | Display delete confirmation dialog | TC_BR84_001 | Verify confirmation dialog MSG59 displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 406 | BR84   | Display delete confirmation dialog | TC_BR84_002 | Verify clicking "No" cancels deletion                              |             |               |               |          |         |          |          |           |               |                     |
| 407 | BR84   | Display delete confirmation dialog | TC_BR84_003 | Verify dialog closes without changes when "No" is clicked          |             |               |               |          |         |          |          |           |               |                     |

#### BR85 - Processing Rules (Delete Dish)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 408 | BR85   | Delete dish from database | TC_BR85_001 | Verify image file "Food/[DishId].jpg" is deleted if exists |             |               |               |          |         |          |          |           |               |                     |
| 409 | BR85   | Delete dish from database | TC_BR85_002 | Verify Delete() method in DishService is called            |             |               |               |          |         |          |          |           |               |                     |
| 410 | BR85   | Delete dish from database | TC_BR85_003 | Verify dish is removed from DishList after deletion        |             |               |               |          |         |          |          |           |               |                     |
| 411 | BR85   | Delete dish from database | TC_BR85_004 | Verify dish is removed from OriginalList after deletion    |             |               |               |          |         |          |          |           |               |                     |
| 412 | BR85   | Delete dish from database | TC_BR85_005 | Verify MSG60 success notification displays after deletion  |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.15: Export Dishes to Excel

#### BR86 - Displaying Rules (Export Dishes)

| No  | Req ID | Req Desc                   | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 413 | BR86   | Display export dishes mode | TC_BR86_001 | Verify IsExporting=TRUE when user selects "Xuất Excel" action           |             |               |               |          |         |          |          |           |               |                     |
| 414 | BR86   | Display export dishes mode | TC_BR86_002 | Verify IsAdding=FALSE, IsEditing=FALSE, IsDeleting=FALSE when exporting |             |               |               |          |         |          |          |           |               |                     |
| 415 | BR86   | Display export dishes mode | TC_BR86_003 | Verify user can apply filter criteria before export                     |             |               |               |          |         |          |          |           |               |                     |
| 416 | BR86   | Display export dishes mode | TC_BR86_004 | Verify ExportToExcelCommand button is visible                           |             |               |               |          |         |          |          |           |               |                     |

#### BR87 - Validation Rules (Export Dishes)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | -------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 417 | BR87   | Validate data before export | TC_BR87_001 | Verify ExportToExcel() checks if DishList has data |             |               |               |          |         |          |          |           |               |                     |
| 418 | BR87   | Validate data before export | TC_BR87_002 | Verify MSG19 displays when DishList is NULL        |             |               |               |          |         |          |          |           |               |                     |
| 419 | BR87   | Validate data before export | TC_BR87_003 | Verify MSG19 displays when DishList.Count = 0      |             |               |               |          |         |          |          |           |               |                     |
| 420 | BR87   | Validate data before export | TC_BR87_004 | Verify export proceeds when DishList has data      |             |               |               |          |         |          |          |           |               |                     |

#### BR88 - Processing Rules (Export Dishes)

| No  | Req ID | Req Desc                       | TC ID       | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ----------- | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 421 | BR88   | Generate Excel file for dishes | TC_BR88_001 | Verify XLWorkbook is created using ClosedXML library            |             |               |               |          |         |          |          |           |               |                     |
| 422 | BR88   | Generate Excel file for dishes | TC_BR88_002 | Verify worksheet "Danh sách Món ăn" is created                  |             |               |               |          |         |          |          |           |               |                     |
| 423 | BR88   | Generate Excel file for dishes | TC_BR88_003 | Verify columns: "Tên món ăn", "Đơn giá", "Ghi chú"              |             |               |               |          |         |          |          |           |               |                     |
| 424 | BR88   | Generate Excel file for dishes | TC_BR88_004 | Verify header formatting: bold, light gray background, centered |             |               |               |          |         |          |          |           |               |                     |
| 425 | BR88   | Generate Excel file for dishes | TC_BR88_005 | Verify number format "#,##0" for price column                   |             |               |               |          |         |          |          |           |               |                     |
| 426 | BR88   | Generate Excel file for dishes | TC_BR88_006 | Verify filename format: "DanhSachMonAn\_[yyyyMMddHHmmss].xlsx"  |             |               |               |          |         |          |          |           |               |                     |
| 427 | BR88   | Generate Excel file for dishes | TC_BR88_007 | Verify MSG61 success notification displays after export         |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.16: View Service Details

#### BR89 - Displaying Rules (ServiceView)

| No  | Req ID | Req Desc                              | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 428 | BR89   | Display ServiceView with service list | TC_BR89_001 | Verify ServiceView displays when user clicks ServiceCommand               |             |               |               |          |         |          |          |           |               |                     |
| 429 | BR89   | Display ServiceView with service list | TC_BR89_002 | Verify database context is reinitialized via resetDatabaseContext()       |             |               |               |          |         |          |          |           |               |                     |
| 430 | BR89   | Display ServiceView with service list | TC_BR89_003 | Verify ServiceViewModel loads services using GetAll() from ServiceService |             |               |               |          |         |          |          |           |               |                     |
| 431 | BR89   | Display ServiceView with service list | TC_BR89_004 | Verify DataGrid displays all services with correct columns                |             |               |               |          |         |          |          |           |               |                     |
| 432 | BR89   | Display ServiceView with service list | TC_BR89_005 | Verify UnitPrice is displayed with proper currency format                 |             |               |               |          |         |          |          |           |               |                     |

#### BR90 - Searching Rules (Service Search)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 433 | BR90   | Filter services by search criteria | TC_BR90_001 | Verify PerformSearch() filters by ServiceName when "Tên dịch vụ" selected |             |               |               |          |         |          |          |           |               |                     |
| 434 | BR90   | Filter services by search criteria | TC_BR90_002 | Verify PerformSearch() filters by UnitPrice when "Đơn giá" selected       |             |               |               |          |         |          |          |           |               |                     |
| 435 | BR90   | Filter services by search criteria | TC_BR90_003 | Verify PerformSearch() filters by Note when "Ghi chú" selected            |             |               |               |          |         |          |          |           |               |                     |
| 436 | BR90   | Filter services by search criteria | TC_BR90_004 | Verify search is case-insensitive                                         |             |               |               |          |         |          |          |           |               |                     |
| 437 | BR90   | Filter services by search criteria | TC_BR90_005 | Verify empty search returns all services                                  |             |               |               |          |         |          |          |           |               |                     |

#### BR91 - Selection Rules (Service Selection)

| No  | Req ID | Req Desc                           | TC ID       | TC Desc                                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ----------- | ----------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 438 | BR91   | Select service and display details | TC_BR91_001 | Verify setSelectedItem triggers when user selects service from DataGrid |             |               |               |          |         |          |          |           |               |                     |
| 439 | BR91   | Select service and display details | TC_BR91_002 | Verify ServiceName field is populated with selected service name        |             |               |               |          |         |          |          |           |               |                     |
| 440 | BR91   | Select service and display details | TC_BR91_003 | Verify UnitPrice field is populated correctly                           |             |               |               |          |         |          |          |           |               |                     |
| 441 | BR91   | Select service and display details | TC_BR91_004 | Verify Note field is populated with selected service note               |             |               |               |          |         |          |          |           |               |                     |
| 442 | BR91   | Select service and display details | TC_BR91_005 | Verify RenderImageAsync() displays service image                        |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.17: Add New Service

#### BR92 - Displaying Rules (Add Service Form)

| No  | Req ID | Req Desc                 | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 443 | BR92   | Display add service form | TC_BR92_001 | Verify IsAdding=TRUE when user selects "Thêm" action               |             |               |               |          |         |          |          |           |               |                     |
| 444 | BR92   | Display add service form | TC_BR92_002 | Verify Reset() clears all form fields and Image=NULL               |             |               |               |          |         |          |          |           |               |                     |
| 445 | BR92   | Display add service form | TC_BR92_003 | Verify form displays ServiceName, UnitPrice, Note, image selection |             |               |               |          |         |          |          |           |               |                     |
| 446 | BR92   | Display add service form | TC_BR92_004 | Verify AddCommand button is visible when IsAdding=TRUE             |             |               |               |          |         |          |          |           |               |                     |

#### BR93 - Validation Rules (Add Service)

| No  | Req ID | Req Desc                            | TC ID       | TC Desc                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ----------- | ----------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 447 | BR93   | Validate service data before saving | TC_BR93_001 | Verify MSG62 displays when ServiceName is empty                   |             |               |               |          |         |          |          |           |               |                     |
| 448 | BR93   | Validate service data before saving | TC_BR93_002 | Verify MSG63 displays when UnitPrice is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 449 | BR93   | Validate service data before saving | TC_BR93_003 | Verify MSG64 displays when UnitPrice is not numeric               |             |               |               |          |         |          |          |           |               |                     |
| 450 | BR93   | Validate service data before saving | TC_BR93_004 | Verify MSG64 displays when UnitPrice < 0                          |             |               |               |          |         |          |          |           |               |                     |
| 451 | BR93   | Validate service data before saving | TC_BR93_005 | Verify MSG65 displays when ServiceName already exists (duplicate) |             |               |               |          |         |          |          |           |               |                     |

#### BR94 - Processing Rules (Add Service Save)

| No  | Req ID | Req Desc                         | TC ID       | TC Desc                                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ----------- | ---------------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 452 | BR94   | Insert new service into database | TC_BR94_001 | Verify ServiceDTO is created with trimmed ServiceName                              |             |               |               |          |         |          |          |           |               |                     |
| 453 | BR94   | Insert new service into database | TC_BR94_002 | Verify UnitPrice is parsed as decimal                                              |             |               |               |          |         |          |          |           |               |                     |
| 454 | BR94   | Insert new service into database | TC_BR94_003 | Verify Create() method in ServiceService is called                                 |             |               |               |          |         |          |          |           |               |                     |
| 455 | BR94   | Insert new service into database | TC_BR94_004 | Verify image cache copied from "Service/Addcache.jpg" to "Service/[ServiceId].jpg" |             |               |               |          |         |          |          |           |               |                     |
| 456 | BR94   | Insert new service into database | TC_BR94_005 | Verify new service is added to ServiceList after creation                          |             |               |               |          |         |          |          |           |               |                     |
| 457 | BR94   | Insert new service into database | TC_BR94_006 | Verify MSG66 success notification displays after save                              |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.18: Edit Service

#### BR95 - Displaying Rules (Edit Service Form)

| No  | Req ID | Req Desc                  | TC ID       | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ----------- | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 458 | BR95   | Display edit service form | TC_BR95_001 | Verify IsEditing=TRUE when user selects "Sửa" action     |             |               |               |          |         |          |          |           |               |                     |
| 459 | BR95   | Display edit service form | TC_BR95_002 | Verify form is populated with selected service data      |             |               |               |          |         |          |          |           |               |                     |
| 460 | BR95   | Display edit service form | TC_BR95_003 | Verify RenderImageAsync() displays current service image |             |               |               |          |         |          |          |           |               |                     |
| 461 | BR95   | Display edit service form | TC_BR95_004 | Verify EditCommand button is visible when IsEditing=TRUE |             |               |               |          |         |          |          |           |               |                     |

#### BR96 - Validation Rules (Edit Service)

| No  | Req ID | Req Desc                     | TC ID       | TC Desc                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------- | ----------- | ----------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 462 | BR96   | Validate edited service data | TC_BR96_001 | Verify CanEdit() returns false when SelectedItem is null          |             |               |               |          |         |          |          |           |               |                     |
| 463 | BR96   | Validate edited service data | TC_BR96_002 | Verify MSG62 displays when ServiceName is empty                   |             |               |               |          |         |          |          |           |               |                     |
| 464 | BR96   | Validate edited service data | TC_BR96_003 | Verify MSG63 displays when UnitPrice is empty                     |             |               |               |          |         |          |          |           |               |                     |
| 465 | BR96   | Validate edited service data | TC_BR96_004 | Verify MSG64 displays when UnitPrice is not numeric or < 0        |             |               |               |          |         |          |          |           |               |                     |
| 466 | BR96   | Validate edited service data | TC_BR96_005 | Verify MSG65 displays when ServiceName duplicates another service |             |               |               |          |         |          |          |           |               |                     |
| 467 | BR96   | Validate edited service data | TC_BR96_006 | Verify MSG16 displays when no changes detected                    |             |               |               |          |         |          |          |           |               |                     |

#### BR97 - Processing Rules (Edit Service Save)

| No  | Req ID | Req Desc                          | TC ID       | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ----------- | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 468 | BR97   | Update service record in database | TC_BR97_001 | Verify existing image deleted when Image=NULL AND image file exists |             |               |               |          |         |          |          |           |               |                     |
| 469 | BR97   | Update service record in database | TC_BR97_002 | Verify ServiceDTO is created with updated values                    |             |               |               |          |         |          |          |           |               |                     |
| 470 | BR97   | Update service record in database | TC_BR97_003 | Verify Update() method in ServiceService is called                  |             |               |               |          |         |          |          |           |               |                     |
| 471 | BR97   | Update service record in database | TC_BR97_004 | Verify image cache copied from "Service/Editcache.jpg"              |             |               |               |          |         |          |          |           |               |                     |
| 472 | BR97   | Update service record in database | TC_BR97_005 | Verify ServiceList is updated at selected index                     |             |               |               |          |         |          |          |           |               |                     |
| 473 | BR97   | Update service record in database | TC_BR97_006 | Verify MSG67 success notification displays after update             |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.19: Delete Service

#### BR98 - Displaying Rules (Delete Service)

| No  | Req ID | Req Desc                    | TC ID       | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ----------- | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 474 | BR98   | Display delete service mode | TC_BR98_001 | Verify IsDeleting=TRUE when user selects "Xóa" action |             |               |               |          |         |          |          |           |               |                     |
| 475 | BR98   | Display delete service mode | TC_BR98_002 | Verify Reset() is called when entering delete mode    |             |               |               |          |         |          |          |           |               |                     |
| 476 | BR98   | Display delete service mode | TC_BR98_003 | Verify DataGrid displays services for selection       |             |               |               |          |         |          |          |           |               |                     |
| 477 | BR98   | Display delete service mode | TC_BR98_004 | Verify user can select service to delete              |             |               |               |          |         |          |          |           |               |                     |

#### BR99 - Reference Check Rules (Delete Service)

| No  | Req ID | Req Desc                               | TC ID       | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------------- | ----------- | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 478 | BR99   | Check service references before delete | TC_BR99_001 | Verify CanDelete() checks if service exists in ServiceDetail table |             |               |               |          |         |          |          |           |               |                     |
| 479 | BR99   | Check service references before delete | TC_BR99_002 | Verify MSG68 displays when service is used in bookings             |             |               |               |          |         |          |          |           |               |                     |
| 480 | BR99   | Check service references before delete | TC_BR99_003 | Verify CanDelete() returns false when service is referenced        |             |               |               |          |         |          |          |           |               |                     |
| 481 | BR99   | Check service references before delete | TC_BR99_004 | Verify user is informed about referenced service details           |             |               |               |          |         |          |          |           |               |                     |

#### BR100 - Confirmation Rules (Delete Service)

| No  | Req ID | Req Desc                           | TC ID        | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ------------ | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 482 | BR100  | Display delete confirmation dialog | TC_BR100_001 | Verify confirmation dialog MSG69 displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 483 | BR100  | Display delete confirmation dialog | TC_BR100_002 | Verify clicking "No" cancels deletion                              |             |               |               |          |         |          |          |           |               |                     |
| 484 | BR100  | Display delete confirmation dialog | TC_BR100_003 | Verify dialog closes without changes when "No" is clicked          |             |               |               |          |         |          |          |           |               |                     |

#### BR101 - Processing Rules (Delete Service Execute)

| No  | Req ID | Req Desc                            | TC ID        | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ------------ | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 485 | BR101  | Delete service record from database | TC_BR101_001 | Verify Delete() method is called in ServiceService when user confirms |             |               |               |          |         |          |          |           |               |                     |
| 486 | BR101  | Delete service record from database | TC_BR101_002 | Verify service image file is deleted from "Service/[ServiceId].jpg"   |             |               |               |          |         |          |          |           |               |                     |
| 487 | BR101  | Delete service record from database | TC_BR101_003 | Verify service is removed from ServiceList collection                 |             |               |               |          |         |          |          |           |               |                     |
| 488 | BR101  | Delete service record from database | TC_BR101_004 | Verify service is removed from OriginalList collection                |             |               |               |          |         |          |          |           |               |                     |
| 489 | BR101  | Delete service record from database | TC_BR101_005 | Verify MSG70 success notification displays after deletion             |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.20: Export Services to Excel

#### BR102 - Displaying Rules (Export Services)

| No  | Req ID | Req Desc                     | TC ID        | TC Desc                                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------- | ------------ | ---------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 490 | BR102  | Display export services mode | TC_BR102_001 | Verify IsExporting=TRUE when user selects "Xuất Excel" action          |             |               |               |          |         |          |          |           |               |                     |
| 491 | BR102  | Display export services mode | TC_BR102_002 | Verify other mode flags set to FALSE (IsAdding, IsEditing, IsDeleting) |             |               |               |          |         |          |          |           |               |                     |
| 492 | BR102  | Display export services mode | TC_BR102_003 | Verify user can apply filter before export                             |             |               |               |          |         |          |          |           |               |                     |
| 493 | BR102  | Display export services mode | TC_BR102_004 | Verify ExportToExcelCommand button is available                        |             |               |               |          |         |          |          |           |               |                     |

#### BR103 - Validation Rules (Export Services)

| No  | Req ID | Req Desc                        | TC ID        | TC Desc                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ------------ | -------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 494 | BR103  | Validate data exists for export | TC_BR103_001 | Verify MSG19 displays when ServiceList is null     |             |               |               |          |         |          |          |           |               |                     |
| 495 | BR103  | Validate data exists for export | TC_BR103_002 | Verify MSG19 displays when ServiceList is empty    |             |               |               |          |         |          |          |           |               |                     |
| 496 | BR103  | Validate data exists for export | TC_BR103_003 | Verify validation passes when ServiceList has data |             |               |               |          |         |          |          |           |               |                     |
| 497 | BR103  | Validate data exists for export | TC_BR103_004 | Verify export proceeds after validation passes     |             |               |               |          |         |          |          |           |               |                     |

#### BR104 - Processing Rules (Export Services to Excel)

| No  | Req ID | Req Desc                            | TC ID        | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ------------ | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 498 | BR104  | Create Excel file with service data | TC_BR104_001 | Verify XLWorkbook is created using ClosedXML library            |             |               |               |          |         |          |          |           |               |                     |
| 499 | BR104  | Create Excel file with service data | TC_BR104_002 | Verify worksheet "Danh sách Dịch vụ" is added                   |             |               |               |          |         |          |          |           |               |                     |
| 500 | BR104  | Create Excel file with service data | TC_BR104_003 | Verify columns created: "Tên dịch vụ", "Đơn giá"                |             |               |               |          |         |          |          |           |               |                     |
| 501 | BR104  | Create Excel file with service data | TC_BR104_004 | Verify all services from ServiceList are exported               |             |               |               |          |         |          |          |           |               |                     |
| 502 | BR104  | Create Excel file with service data | TC_BR104_005 | Verify header formatting: bold, light gray background, centered |             |               |               |          |         |          |          |           |               |                     |
| 503 | BR104  | Create Excel file with service data | TC_BR104_006 | Verify filename format "DanhSachDichVu\_[yyyyMMddHHmmss].xlsx"  |             |               |               |          |         |          |          |           |               |                     |
| 504 | BR104  | Create Excel file with service data | TC_BR104_007 | Verify SaveFileDialog opens for user to select location         |             |               |               |          |         |          |          |           |               |                     |

---

## 3. Shift Management Use Cases

### UC 2.1.3.21: View Shift Details

#### BR105 - Displaying Rules (ShiftView)

| No  | Req ID | Req Desc                 | TC ID        | TC Desc                                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ------------ | ----------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 505 | BR105  | Display ShiftView screen | TC_BR105_001 | Verify ShiftView displays when user has "Shift" permission                    |             |               |               |          |         |          |          |           |               |                     |
| 506 | BR105  | Display ShiftView screen | TC_BR105_002 | Verify DataGrid control displays shift data                                   |             |               |               |          |         |          |          |           |               |                     |
| 507 | BR105  | Display ShiftView screen | TC_BR105_003 | Verify SelectedAction ComboBox shows actions: Xem, Thêm, Sửa, Xóa, Xuất Excel |             |               |               |          |         |          |          |           |               |                     |
| 508 | BR105  | Display ShiftView screen | TC_BR105_004 | Verify TextBox controls for input: ShiftName, StartTime, EndTime              |             |               |               |          |         |          |          |           |               |                     |
| 509 | BR105  | Display ShiftView screen | TC_BR105_005 | Verify SearchTextBox for filtering shifts                                     |             |               |               |          |         |          |          |           |               |                     |

#### BR106 - Querying Rules (Load Shifts)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 510 | BR106  | Query Shift table for all records | TC_BR106_001 | Verify GetAll() method loads all shifts from database         |             |               |               |          |         |          |          |           |               |                     |
| 511 | BR106  | Query Shift table for all records | TC_BR106_002 | Verify ShiftList ObservableCollection is populated            |             |               |               |          |         |          |          |           |               |                     |
| 512 | BR106  | Query Shift table for all records | TC_BR106_003 | Verify OriginalList stores backup of all shifts               |             |               |               |          |         |          |          |           |               |                     |
| 513 | BR106  | Query Shift table for all records | TC_BR106_004 | Verify empty list handled gracefully with appropriate message |             |               |               |          |         |          |          |           |               |                     |

#### BR107 - Selection Rules (Select Shift)

| No  | Req ID | Req Desc                     | TC ID        | TC Desc                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------- | ------------ | ----------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 514 | BR107  | Select shift to view details | TC_BR107_001 | Verify SelectedItem is set when user clicks on DataGrid row |             |               |               |          |         |          |          |           |               |                     |
| 515 | BR107  | Select shift to view details | TC_BR107_002 | Verify ShiftName field displays selected shift name         |             |               |               |          |         |          |          |           |               |                     |
| 516 | BR107  | Select shift to view details | TC_BR107_003 | Verify StartTime field displays in "HH:mm" format           |             |               |               |          |         |          |          |           |               |                     |
| 517 | BR107  | Select shift to view details | TC_BR107_004 | Verify EndTime field displays in "HH:mm" format             |             |               |               |          |         |          |          |           |               |                     |
| 518 | BR107  | Select shift to view details | TC_BR107_005 | Verify selection change triggers PropertyChanged event      |             |               |               |          |         |          |          |           |               |                     |

#### BR108 - Searching Rules (Filter Shifts)

| No  | Req ID | Req Desc                     | TC ID        | TC Desc                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------- | ------------ | ----------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 519 | BR108  | Filter shifts by search text | TC_BR108_001 | Verify search filters by ShiftName containing search text   |             |               |               |          |         |          |          |           |               |                     |
| 520 | BR108  | Filter shifts by search text | TC_BR108_002 | Verify search is case-insensitive                           |             |               |               |          |         |          |          |           |               |                     |
| 521 | BR108  | Filter shifts by search text | TC_BR108_003 | Verify empty search text shows all shifts from OriginalList |             |               |               |          |         |          |          |           |               |                     |
| 522 | BR108  | Filter shifts by search text | TC_BR108_004 | Verify real-time filtering as user types                    |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.22: Add New Shift

#### BR109 - Displaying Rules (Add Shift Form)

| No  | Req ID | Req Desc               | TC ID        | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------- | ------------ | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 523 | BR109  | Display add shift form | TC_BR109_001 | Verify IsAdding=TRUE when user selects "Thêm" action       |             |               |               |          |         |          |          |           |               |                     |
| 524 | BR109  | Display add shift form | TC_BR109_002 | Verify Reset() clears all input fields                     |             |               |               |          |         |          |          |           |               |                     |
| 525 | BR109  | Display add shift form | TC_BR109_003 | Verify AddCommand button is visible when IsAdding=TRUE     |             |               |               |          |         |          |          |           |               |                     |
| 526 | BR109  | Display add shift form | TC_BR109_004 | Verify TimePicker controls for StartTime and EndTime input |             |               |               |          |         |          |          |           |               |                     |

#### BR110 - Validation Rules (Add Shift)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | ---------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 527 | BR110  | Validate shift input data | TC_BR110_001 | Verify MSG71 displays when ShiftName is empty        |             |               |               |          |         |          |          |           |               |                     |
| 528 | BR110  | Validate shift input data | TC_BR110_002 | Verify MSG72 displays when StartTime is not selected |             |               |               |          |         |          |          |           |               |                     |
| 529 | BR110  | Validate shift input data | TC_BR110_003 | Verify MSG73 displays when EndTime is not selected   |             |               |               |          |         |          |          |           |               |                     |
| 530 | BR110  | Validate shift input data | TC_BR110_004 | Verify MSG74 displays when EndTime <= StartTime      |             |               |               |          |         |          |          |           |               |                     |
| 531 | BR110  | Validate shift input data | TC_BR110_005 | Verify MSG75 displays when ShiftName already exists  |             |               |               |          |         |          |          |           |               |                     |

#### BR111 - Processing Rules (Add Shift Save)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 532 | BR111  | Insert new shift into database | TC_BR111_001 | Verify ShiftDTO is created with trimmed ShiftName     |             |               |               |          |         |          |          |           |               |                     |
| 533 | BR111  | Insert new shift into database | TC_BR111_002 | Verify StartTime and EndTime are stored as TimeSpan   |             |               |               |          |         |          |          |           |               |                     |
| 534 | BR111  | Insert new shift into database | TC_BR111_003 | Verify Create() method in ShiftService is called      |             |               |               |          |         |          |          |           |               |                     |
| 535 | BR111  | Insert new shift into database | TC_BR111_004 | Verify new shift is added to ShiftList after creation |             |               |               |          |         |          |          |           |               |                     |
| 536 | BR111  | Insert new shift into database | TC_BR111_005 | Verify MSG76 success notification displays after save |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.23: Edit Shift

#### BR112 - Displaying Rules (Edit Shift Form)

| No  | Req ID | Req Desc                | TC ID        | TC Desc                                              | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------- | ------------ | ---------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 537 | BR112  | Display edit shift form | TC_BR112_001 | Verify IsEditing=TRUE when user selects "Sửa" action |             |               |               |          |         |          |          |           |               |                     |
| 538 | BR112  | Display edit shift form | TC_BR112_002 | Verify form is populated with selected shift data    |             |               |               |          |         |          |          |           |               |                     |
| 539 | BR112  | Display edit shift form | TC_BR112_003 | Verify StartTime TimePicker shows current value      |             |               |               |          |         |          |          |           |               |                     |
| 540 | BR112  | Display edit shift form | TC_BR112_004 | Verify EndTime TimePicker shows current value        |             |               |               |          |         |          |          |           |               |                     |

#### BR113 - Validation Rules (Edit Shift)

| No  | Req ID | Req Desc                   | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 541 | BR113  | Validate edited shift data | TC_BR113_001 | Verify CanEdit() returns false when SelectedItem is null      |             |               |               |          |         |          |          |           |               |                     |
| 542 | BR113  | Validate edited shift data | TC_BR113_002 | Verify MSG71 displays when ShiftName is empty                 |             |               |               |          |         |          |          |           |               |                     |
| 543 | BR113  | Validate edited shift data | TC_BR113_003 | Verify MSG74 displays when EndTime <= StartTime               |             |               |               |          |         |          |          |           |               |                     |
| 544 | BR113  | Validate edited shift data | TC_BR113_004 | Verify MSG75 displays when ShiftName duplicates another shift |             |               |               |          |         |          |          |           |               |                     |
| 545 | BR113  | Validate edited shift data | TC_BR113_005 | Verify MSG16 displays when no changes detected                |             |               |               |          |         |          |          |           |               |                     |

#### BR114 - Displaying Rules (Delete Shift)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 546 | BR114  | Display delete shift mode | TC_BR114_001 | Verify IsDeleting=TRUE when user selects "Xóa" action |             |               |               |          |         |          |          |           |               |                     |
| 547 | BR114  | Display delete shift mode | TC_BR114_002 | Verify Reset() is called when entering delete mode    |             |               |               |          |         |          |          |           |               |                     |
| 548 | BR114  | Display delete shift mode | TC_BR114_003 | Verify DataGrid displays shifts for selection         |             |               |               |          |         |          |          |           |               |                     |
| 549 | BR114  | Display delete shift mode | TC_BR114_004 | Verify user can select shift to delete                |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.24: Delete Shift

#### BR115 - Reference Check Rules (Delete Shift)

| No  | Req ID | Req Desc                             | TC ID        | TC Desc                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ------------ | ----------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 550 | BR115  | Check shift references before delete | TC_BR115_001 | Verify CanDelete() checks if shift exists in Booking table        |             |               |               |          |         |          |          |           |               |                     |
| 551 | BR115  | Check shift references before delete | TC_BR115_002 | Verify MSG80 displays when shift is used in bookings              |             |               |               |          |         |          |          |           |               |                     |
| 552 | BR115  | Check shift references before delete | TC_BR115_003 | Verify CanDelete() returns false when shift is referenced         |             |               |               |          |         |          |          |           |               |                     |
| 553 | BR115  | Check shift references before delete | TC_BR115_004 | Verify user is informed about number of bookings using this shift |             |               |               |          |         |          |          |           |               |                     |
| 554 | BR115  | Check shift references before delete | TC_BR115_005 | Verify CanDelete() returns true when shift has no references      |             |               |               |          |         |          |          |           |               |                     |

#### BR116 - Confirmation Rules (Delete Shift)

| No  | Req ID | Req Desc                           | TC ID        | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ------------ | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 555 | BR116  | Display delete confirmation dialog | TC_BR116_001 | Verify confirmation dialog MSG81 displays when no references exist |             |               |               |          |         |          |          |           |               |                     |
| 556 | BR116  | Display delete confirmation dialog | TC_BR116_002 | Verify clicking "No" cancels deletion                              |             |               |               |          |         |          |          |           |               |                     |
| 557 | BR116  | Display delete confirmation dialog | TC_BR116_003 | Verify dialog closes without changes when "No" is clicked          |             |               |               |          |         |          |          |           |               |                     |
| 558 | BR116  | Display delete confirmation dialog | TC_BR116_004 | Verify "Yes" button proceeds to delete execution                   |             |               |               |          |         |          |          |           |               |                     |

#### BR117 - Processing Rules (Delete Shift Execute)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 559 | BR117  | Delete shift record from database | TC_BR117_001 | Verify Delete() method is called in ShiftService when user confirms |             |               |               |          |         |          |          |           |               |                     |
| 560 | BR117  | Delete shift record from database | TC_BR117_002 | Verify SQL DELETE statement executed on Shift table                 |             |               |               |          |         |          |          |           |               |                     |
| 561 | BR117  | Delete shift record from database | TC_BR117_003 | Verify shift is removed from ShiftList collection                   |             |               |               |          |         |          |          |           |               |                     |
| 562 | BR117  | Delete shift record from database | TC_BR117_004 | Verify shift is removed from OriginalList collection                |             |               |               |          |         |          |          |           |               |                     |
| 563 | BR117  | Delete shift record from database | TC_BR117_005 | Verify MSG83 success notification displays after deletion           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.3.25: Export Shifts to Excel

#### BR118 - Displaying Rules (Export Shifts)

| No  | Req ID | Req Desc                   | TC ID        | TC Desc                                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ------------ | ---------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 564 | BR118  | Display export shifts mode | TC_BR118_001 | Verify IsExporting=TRUE when user selects "Xuất Excel" action          |             |               |               |          |         |          |          |           |               |                     |
| 565 | BR118  | Display export shifts mode | TC_BR118_002 | Verify other mode flags set to FALSE (IsAdding, IsEditing, IsDeleting) |             |               |               |          |         |          |          |           |               |                     |
| 566 | BR118  | Display export shifts mode | TC_BR118_003 | Verify user can apply filter criteria before export                    |             |               |               |          |         |          |          |           |               |                     |
| 567 | BR118  | Display export shifts mode | TC_BR118_004 | Verify ExportToExcelCommand button is available                        |             |               |               |          |         |          |          |           |               |                     |

#### BR119 - Validation Rules (Export Shifts)

| No  | Req ID | Req Desc                        | TC ID        | TC Desc                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ------------ | ------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 568 | BR119  | Validate data exists for export | TC_BR119_001 | Verify MSG19 displays when ShiftList is null     |             |               |               |          |         |          |          |           |               |                     |
| 569 | BR119  | Validate data exists for export | TC_BR119_002 | Verify MSG19 displays when ShiftList is empty    |             |               |               |          |         |          |          |           |               |                     |
| 570 | BR119  | Validate data exists for export | TC_BR119_003 | Verify validation passes when ShiftList has data |             |               |               |          |         |          |          |           |               |                     |
| 571 | BR119  | Validate data exists for export | TC_BR119_004 | Verify export proceeds after validation passes   |             |               |               |          |         |          |          |           |               |                     |

#### BR120 - Processing Rules (Export Shifts to Excel)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 572 | BR120  | Create Excel file with shift data | TC_BR120_001 | Verify XLWorkbook is created using ClosedXML library                |             |               |               |          |         |          |          |           |               |                     |
| 573 | BR120  | Create Excel file with shift data | TC_BR120_002 | Verify worksheet "Danh sách Ca" is added                            |             |               |               |          |         |          |          |           |               |                     |
| 574 | BR120  | Create Excel file with shift data | TC_BR120_003 | Verify columns: "Tên ca", "Thời gian bắt đầu", "Thời gian kết thúc" |             |               |               |          |         |          |          |           |               |                     |
| 575 | BR120  | Create Excel file with shift data | TC_BR120_004 | Verify all shifts from ShiftList are exported                       |             |               |               |          |         |          |          |           |               |                     |
| 576 | BR120  | Create Excel file with shift data | TC_BR120_005 | Verify time format "HH:mm" for StartTime and EndTime columns        |             |               |               |          |         |          |          |           |               |                     |
| 577 | BR120  | Create Excel file with shift data | TC_BR120_006 | Verify header formatting: bold, light gray background, centered     |             |               |               |          |         |          |          |           |               |                     |
| 578 | BR120  | Create Excel file with shift data | TC_BR120_007 | Verify filename format "DanhSachCa\_[yyyyMMddHHmmss].xlsx"          |             |               |               |          |         |          |          |           |               |                     |

---

## 4. Customer Booking Operations

### UC 2.1.4.2: Check Hall Availability

#### BR121 - Displaying Rules (Hall Availability Page)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 579 | BR121  | Display hall availability page | TC_BR121_001 | Verify hall availability page displays for logged-in customer |             |               |               |          |         |          |          |           |               |                     |
| 580 | BR121  | Display hall availability page | TC_BR121_002 | Verify calendar control for EventDate selection               |             |               |               |          |         |          |          |           |               |                     |
| 581 | BR121  | Display hall availability page | TC_BR121_003 | Verify Shift dropdown is populated with active shifts         |             |               |               |          |         |          |          |           |               |                     |
| 582 | BR121  | Display hall availability page | TC_BR121_004 | Verify Check Availability button is visible                   |             |               |               |          |         |          |          |           |               |                     |
| 583 | BR121  | Display hall availability page | TC_BR121_005 | Verify results area is initially empty                        |             |               |               |          |         |          |          |           |               |                     |

#### BR122 - Validation Rules (Check Availability)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 584 | BR122  | Validate check availability input | TC_BR122_001 | Verify MSG84 displays when EventDate is in the past          |             |               |               |          |         |          |          |           |               |                     |
| 585 | BR122  | Validate check availability input | TC_BR122_002 | Verify MSG85 displays when Shift is not selected             |             |               |               |          |         |          |          |           |               |                     |
| 586 | BR122  | Validate check availability input | TC_BR122_003 | Verify validation passes with future date and selected shift |             |               |               |          |         |          |          |           |               |                     |
| 587 | BR122  | Validate check availability input | TC_BR122_004 | Verify today's date is acceptable for availability check     |             |               |               |          |         |          |          |           |               |                     |

#### BR123 - Processing Rules (Query Available Halls)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ----------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 588 | BR123  | Query and display available halls | TC_BR123_001 | Verify query excludes halls with existing bookings for date/shift |             |               |               |          |         |          |          |           |               |                     |
| 589 | BR123  | Query and display available halls | TC_BR123_002 | Verify available halls display with name, type, capacity          |             |               |               |          |         |          |          |           |               |                     |
| 590 | BR123  | Query and display available halls | TC_BR123_003 | Verify hall pricing information is displayed                      |             |               |               |          |         |          |          |           |               |                     |
| 591 | BR123  | Query and display available halls | TC_BR123_004 | Verify MSG86 displays when no halls are available                 |             |               |               |          |         |          |          |           |               |                     |
| 592 | BR123  | Query and display available halls | TC_BR123_005 | Verify Book Now button available for each hall result             |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.4.3: Submit Wedding Reservation

#### BR124 - Displaying Rules (Booking Form)

| No  | Req ID | Req Desc             | TC ID        | TC Desc                                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------- | ------------ | ------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 593 | BR124  | Display booking form | TC_BR124_001 | Verify booking form displays when customer clicks "Đặt tiệc" |             |               |               |          |         |          |          |           |               |                     |
| 594 | BR124  | Display booking form | TC_BR124_002 | Verify GroomName and BrideName input fields are visible      |             |               |               |          |         |          |          |           |               |                     |
| 595 | BR124  | Display booking form | TC_BR124_003 | Verify PhoneNumber input field is visible                    |             |               |               |          |         |          |          |           |               |                     |
| 596 | BR124  | Display booking form | TC_BR124_004 | Verify TableCount input field is visible                     |             |               |               |          |         |          |          |           |               |                     |
| 597 | BR124  | Display booking form | TC_BR124_005 | Verify selected hall/date/shift info is displayed read-only  |             |               |               |          |         |          |          |           |               |                     |

#### BR125 - Validation Rules (Submit Booking)

| No  | Req ID | Req Desc                         | TC ID        | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ------------ | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 598 | BR125  | Validate booking submission data | TC_BR125_001 | Verify MSG87 displays when GroomName is empty              |             |               |               |          |         |          |          |           |               |                     |
| 599 | BR125  | Validate booking submission data | TC_BR125_002 | Verify MSG87 displays when BrideName is empty              |             |               |               |          |         |          |          |           |               |                     |
| 600 | BR125  | Validate booking submission data | TC_BR125_003 | Verify MSG88 displays when PhoneNumber is empty            |             |               |               |          |         |          |          |           |               |                     |
| 601 | BR125  | Validate booking submission data | TC_BR125_004 | Verify MSG88 displays when PhoneNumber format is invalid   |             |               |               |          |         |          |          |           |               |                     |
| 602 | BR125  | Validate booking submission data | TC_BR125_005 | Verify MSG89 displays when TableCount <= 0                 |             |               |               |          |         |          |          |           |               |                     |
| 603 | BR125  | Validate booking submission data | TC_BR125_006 | Verify MSG89 displays when TableCount > Hall.MaxTableCount |             |               |               |          |         |          |          |           |               |                     |

#### BR126 - Processing Rules (Create Booking)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 604 | BR126  | Create new booking record | TC_BR126_001 | Verify Booking record created with status = "Pending"           |             |               |               |          |         |          |          |           |               |                     |
| 605 | BR126  | Create new booking record | TC_BR126_002 | Verify estimated total = TableCount × Hall.MinTablePrice        |             |               |               |          |         |          |          |           |               |                     |
| 606 | BR126  | Create new booking record | TC_BR126_003 | Verify booking inserted into database                           |             |               |               |          |         |          |          |           |               |                     |
| 607 | BR126  | Create new booking record | TC_BR126_004 | Verify confirmation email sent to customer                      |             |               |               |          |         |          |          |           |               |                     |
| 608 | BR126  | Create new booking record | TC_BR126_005 | Verify MSG90 success notification with booking reference number |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.4.4: View My Booking Details

#### BR127 - Displaying Rules (My Bookings Page)

| No  | Req ID | Req Desc                 | TC ID        | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ------------ | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 609 | BR127  | Display my bookings list | TC_BR127_001 | Verify customer's bookings page displays                 |             |               |               |          |         |          |          |           |               |                     |
| 610 | BR127  | Display my bookings list | TC_BR127_002 | Verify query filters by current customer ID              |             |               |               |          |         |          |          |           |               |                     |
| 611 | BR127  | Display my bookings list | TC_BR127_003 | Verify bookings ordered by CreatedDate DESC              |             |               |               |          |         |          |          |           |               |                     |
| 612 | BR127  | Display my bookings list | TC_BR127_004 | Verify status indicators (color coding) for each booking |             |               |               |          |         |          |          |           |               |                     |
| 613 | BR127  | Display my bookings list | TC_BR127_005 | Verify empty state message when customer has no bookings |             |               |               |          |         |          |          |           |               |                     |

#### BR128 - Selection Rules (View Booking Details)

| No  | Req ID | Req Desc                         | TC ID        | TC Desc                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ------------ | --------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 614 | BR128  | Display selected booking details | TC_BR128_001 | Verify full booking details display when booking selected |             |               |               |          |         |          |          |           |               |                     |
| 615 | BR128  | Display selected booking details | TC_BR128_002 | Verify hall information section displays                  |             |               |               |          |         |          |          |           |               |                     |
| 616 | BR128  | Display selected booking details | TC_BR128_003 | Verify event date and shift information displays          |             |               |               |          |         |          |          |           |               |                     |
| 617 | BR128  | Display selected booking details | TC_BR128_004 | Verify menu items list displays (if any)                  |             |               |               |          |         |          |          |           |               |                     |
| 618 | BR128  | Display selected booking details | TC_BR128_005 | Verify services list displays (if any)                    |             |               |               |          |         |          |          |           |               |                     |
| 619 | BR128  | Display selected booking details | TC_BR128_006 | Verify total amount and payment status displays           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.4.5: Edit My Booking Request

#### BR129 - Displaying Rules (Edit Booking Form)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 620 | BR129  | Display edit booking form | TC_BR129_001 | Verify edit form displays for booking with status = "Pending" |             |               |               |          |         |          |          |           |               |                     |
| 621 | BR129  | Display edit booking form | TC_BR129_002 | Verify MSG91 warning displays when status != "Pending"        |             |               |               |          |         |          |          |           |               |                     |
| 622 | BR129  | Display edit booking form | TC_BR129_003 | Verify form populated with current booking values             |             |               |               |          |         |          |          |           |               |                     |
| 623 | BR129  | Display edit booking form | TC_BR129_004 | Verify edit disabled for non-pending bookings                 |             |               |               |          |         |          |          |           |               |                     |

#### BR130 - Validation Rules (Edit Booking)

| No  | Req ID | Req Desc                   | TC ID        | TC Desc                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ------------ | ---------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 624 | BR130  | Validate edit booking data | TC_BR130_001 | Verify MSG87 displays when GroomName is empty  |             |               |               |          |         |          |          |           |               |                     |
| 625 | BR130  | Validate edit booking data | TC_BR130_002 | Verify MSG87 displays when BrideName is empty  |             |               |               |          |         |          |          |           |               |                     |
| 626 | BR130  | Validate edit booking data | TC_BR130_003 | Verify MSG88 displays when PhoneNumber invalid |             |               |               |          |         |          |          |           |               |                     |
| 627 | BR130  | Validate edit booking data | TC_BR130_004 | Verify MSG89 displays when TableCount invalid  |             |               |               |          |         |          |          |           |               |                     |
| 628 | BR130  | Validate edit booking data | TC_BR130_005 | Verify validation passes with all valid data   |             |               |               |          |         |          |          |           |               |                     |

#### BR131 - Processing Rules (Update Booking)

| No  | Req ID | Req Desc              | TC ID        | TC Desc                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------- | ------------ | ------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 629 | BR131  | Update booking record | TC_BR131_001 | Verify Booking record updated in database        |             |               |               |          |         |          |          |           |               |                     |
| 630 | BR131  | Update booking record | TC_BR131_002 | Verify estimated total recalculated after update |             |               |               |          |         |          |          |           |               |                     |
| 631 | BR131  | Update booking record | TC_BR131_003 | Verify MSG92 success notification displays       |             |               |               |          |         |          |          |           |               |                     |
| 632 | BR131  | Update booking record | TC_BR131_004 | Verify booking list refreshes after update       |             |               |               |          |         |          |          |           |               |                     |
| 633 | BR131  | Update booking record | TC_BR131_005 | Verify modification timestamp updated            |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.4.6: Cancel My Booking

#### BR132 - Displaying Rules (Cancel Booking)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 634 | BR132  | Display cancel booking options | TC_BR132_001 | Verify cancel button visible for eligible bookings              |             |               |               |          |         |          |          |           |               |                     |
| 635 | BR132  | Display cancel booking options | TC_BR132_002 | Verify MSG93 warning displays when booking status = "Completed" |             |               |               |          |         |          |          |           |               |                     |
| 636 | BR132  | Display cancel booking options | TC_BR132_003 | Verify MSG93 warning displays when booking status = "Cancelled" |             |               |               |          |         |          |          |           |               |                     |
| 637 | BR132  | Display cancel booking options | TC_BR132_004 | Verify cancel button hidden for ineligible bookings             |             |               |               |          |         |          |          |           |               |                     |

#### BR133 - Validation Rules (Cancel Eligibility)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ---------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 638 | BR133  | Validate cancellation eligibility | TC_BR133_001 | Verify cancellation allowed for "Pending" status bookings        |             |               |               |          |         |          |          |           |               |                     |
| 639 | BR133  | Validate cancellation eligibility | TC_BR133_002 | Verify cancellation allowed for "Confirmed" status within period |             |               |               |          |         |          |          |           |               |                     |
| 640 | BR133  | Validate cancellation eligibility | TC_BR133_003 | Verify cancellation blocked for "Completed" bookings             |             |               |               |          |         |          |          |           |               |                     |
| 641 | BR133  | Validate cancellation eligibility | TC_BR133_004 | Verify cancellation blocked for already "Cancelled" bookings     |             |               |               |          |         |          |          |           |               |                     |

#### BR134 - Penalty Calculation Rules (Cancel Booking)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 642 | BR134  | Calculate cancellation penalty | TC_BR134_001 | Verify no penalty when no deposit paid                             |             |               |               |          |         |          |          |           |               |                     |
| 643 | BR134  | Calculate cancellation penalty | TC_BR134_002 | Verify penalty = DepositAmount × PenaltyRate when within threshold |             |               |               |          |         |          |          |           |               |                     |
| 644 | BR134  | Calculate cancellation penalty | TC_BR134_003 | Verify MSG96 displays penalty amount in confirmation dialog        |             |               |               |          |         |          |          |           |               |                     |
| 645 | BR134  | Calculate cancellation penalty | TC_BR134_004 | Verify penalty calculation uses Parameter.PenaltyRate              |             |               |               |          |         |          |          |           |               |                     |
| 646 | BR134  | Calculate cancellation penalty | TC_BR134_005 | Verify penalty calculated based on days before EventDate           |             |               |               |          |         |          |          |           |               |                     |

#### BR135 - Confirmation Rules (Cancel Booking)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 647 | BR135  | Display cancellation confirmation | TC_BR135_001 | Verify MSG96 confirmation dialog displays        |             |               |               |          |         |          |          |           |               |                     |
| 648 | BR135  | Display cancellation confirmation | TC_BR135_002 | Verify clicking "No" cancels the operation       |             |               |               |          |         |          |          |           |               |                     |
| 649 | BR135  | Display cancellation confirmation | TC_BR135_003 | Verify penalty amount shown in confirmation      |             |               |               |          |         |          |          |           |               |                     |
| 650 | BR135  | Display cancellation confirmation | TC_BR135_004 | Verify "Yes" proceeds to cancellation processing |             |               |               |          |         |          |          |           |               |                     |

#### BR136 - Processing Rules (Execute Cancellation)

| No  | Req ID | Req Desc                     | TC ID        | TC Desc                                      | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------- | ------------ | -------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 651 | BR136  | Execute booking cancellation | TC_BR136_001 | Verify Booking.Status updated to "Cancelled" |             |               |               |          |         |          |          |           |               |                     |
| 652 | BR136  | Execute booking cancellation | TC_BR136_002 | Verify cancellation reason recorded          |             |               |               |          |         |          |          |           |               |                     |
| 653 | BR136  | Execute booking cancellation | TC_BR136_003 | Verify penalty invoice created if applicable |             |               |               |          |         |          |          |           |               |                     |
| 654 | BR136  | Execute booking cancellation | TC_BR136_004 | Verify cancellation confirmation email sent  |             |               |               |          |         |          |          |           |               |                     |
| 655 | BR136  | Execute booking cancellation | TC_BR136_005 | Verify MSG98 success notification displays   |             |               |               |          |         |          |          |           |               |                     |

---

## 5. Staff Booking Management

### UC 2.1.5.1: Check System Hall Availability

#### BR137 - Displaying Rules (Staff Hall Availability)

| No  | Req ID | Req Desc                              | TC ID        | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------- | ------------ | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 656 | BR137  | Display staff hall availability check | TC_BR137_001 | Verify booking management screen displays for authorized users |             |               |               |          |         |          |          |           |               |                     |
| 657 | BR137  | Display staff hall availability check | TC_BR137_002 | Verify calendar view control for date selection                |             |               |               |          |         |          |          |           |               |                     |
| 658 | BR137  | Display staff hall availability check | TC_BR137_003 | Verify shift selection dropdown available                      |             |               |               |          |         |          |          |           |               |                     |
| 659 | BR137  | Display staff hall availability check | TC_BR137_004 | Verify hall capacity filter option available                   |             |               |               |          |         |          |          |           |               |                     |

#### BR138 - Processing Rules (Query Staff Hall Availability)

| No  | Req ID | Req Desc                        | TC ID        | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------- | ------------ | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 660 | BR138  | Query available halls for staff | TC_BR138_001 | Verify query excludes cancelled bookings from availability |             |               |               |          |         |          |          |           |               |                     |
| 661 | BR138  | Query available halls for staff | TC_BR138_002 | Verify hall details display: name, type, capacity, pricing |             |               |               |          |         |          |          |           |               |                     |
| 662 | BR138  | Query available halls for staff | TC_BR138_003 | Verify MSG90 displays when no halls available              |             |               |               |          |         |          |          |           |               |                     |
| 663 | BR138  | Query available halls for staff | TC_BR138_004 | Verify "Create Booking" button available for each hall     |             |               |               |          |         |          |          |           |               |                     |
| 664 | BR138  | Query available halls for staff | TC_BR138_005 | Verify existing bookings shown for occupied halls          |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.5.2: Create Booking for Customer

#### BR139 - Displaying Rules (Staff Create Booking)

| No  | Req ID | Req Desc                   | TC ID        | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------- | ------------ | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 665 | BR139  | Display staff booking form | TC_BR139_001 | Verify booking form displays when staff clicks "Tạo phiếu đặt"        |             |               |               |          |         |          |          |           |               |                     |
| 666 | BR139  | Display staff booking form | TC_BR139_002 | Verify customer info fields: GroomName, BrideName, PhoneNumber        |             |               |               |          |         |          |          |           |               |                     |
| 667 | BR139  | Display staff booking form | TC_BR139_003 | Verify event selection fields: EventDate, Shift, Hall                 |             |               |               |          |         |          |          |           |               |                     |
| 668 | BR139  | Display staff booking form | TC_BR139_004 | Verify booking details: TableCount, Menu selection, Service selection |             |               |               |          |         |          |          |           |               |                     |
| 669 | BR139  | Display staff booking form | TC_BR139_005 | Verify total amount auto-calculated as fields are filled              |             |               |               |          |         |          |          |           |               |                     |

#### BR140 - Validation Rules (Staff Create Booking)

| No  | Req ID | Req Desc                    | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 670 | BR140  | Validate staff booking data | TC_BR140_001 | Verify MSG10 displays when required fields are empty          |             |               |               |          |         |          |          |           |               |                     |
| 671 | BR140  | Validate staff booking data | TC_BR140_002 | Verify MSG91 displays when TableCount exceeds hall capacity   |             |               |               |          |         |          |          |           |               |                     |
| 672 | BR140  | Validate staff booking data | TC_BR140_003 | Verify MSG102 displays when hall not available for date/shift |             |               |               |          |         |          |          |           |               |                     |
| 673 | BR140  | Validate staff booking data | TC_BR140_004 | Verify phone number format validation                         |             |               |               |          |         |          |          |           |               |                     |
| 674 | BR140  | Validate staff booking data | TC_BR140_005 | Verify minimum table count validation                         |             |               |               |          |         |          |          |           |               |                     |

#### BR141 - Processing Rules (Staff Create Booking)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | --------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 675 | BR141  | Create booking record by staff | TC_BR141_001 | Verify total = TableCount × MinTablePrice + MenuTotal + ServicesTotal |             |               |               |          |         |          |          |           |               |                     |
| 676 | BR141  | Create booking record by staff | TC_BR141_002 | Verify Booking created with status = "Confirmed"                      |             |               |               |          |         |          |          |           |               |                     |
| 677 | BR141  | Create booking record by staff | TC_BR141_003 | Verify invoice auto-generated for booking                             |             |               |               |          |         |          |          |           |               |                     |
| 678 | BR141  | Create booking record by staff | TC_BR141_004 | Verify MSG103 success notification with booking ID                    |             |               |               |          |         |          |          |           |               |                     |
| 679 | BR141  | Create booking record by staff | TC_BR141_005 | Verify booking list refreshes after creation                          |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.5.3: Delete Booking

#### BR142 - Displaying Rules (Delete Booking)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                           | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 680 | BR142  | Display delete booking options | TC_BR142_001 | Verify delete button visible for selected booking |             |               |               |          |         |          |          |           |               |                     |
| 681 | BR142  | Display delete booking options | TC_BR142_002 | Verify booking details shown before delete        |             |               |               |          |         |          |          |           |               |                     |
| 682 | BR142  | Display delete booking options | TC_BR142_003 | Verify payment status displayed in delete view    |             |               |               |          |         |          |          |           |               |                     |

#### BR143 - Validation Rules (Delete Booking)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | -------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 683 | BR143  | Validate booking deletion | TC_BR143_001 | Verify warning when booking has payments (deposit/full)        |             |               |               |          |         |          |          |           |               |                     |
| 684 | BR143  | Validate booking deletion | TC_BR143_002 | Verify MSG104 error when booking status = "Completed"          |             |               |               |          |         |          |          |           |               |                     |
| 685 | BR143  | Validate booking deletion | TC_BR143_003 | Verify refund requirement notification for paid bookings       |             |               |               |          |         |          |          |           |               |                     |
| 686 | BR143  | Validate booking deletion | TC_BR143_004 | Verify deletion allowed for "Pending" bookings without payment |             |               |               |          |         |          |          |           |               |                     |

#### BR144 - Processing Rules (Delete Booking)

| No  | Req ID | Req Desc                 | TC ID        | TC Desc                                               | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ------------ | ----------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 687 | BR144  | Execute booking deletion | TC_BR144_001 | Verify Booking.Status updated to "Deleted" or removed |             |               |               |          |         |          |          |           |               |                     |
| 688 | BR144  | Execute booking deletion | TC_BR144_002 | Verify deletion reason logged                         |             |               |               |          |         |          |          |           |               |                     |
| 689 | BR144  | Execute booking deletion | TC_BR144_003 | Verify MSG105 success notification displays           |             |               |               |          |         |          |          |           |               |                     |
| 690 | BR144  | Execute booking deletion | TC_BR144_004 | Verify booking list refreshes after deletion          |             |               |               |          |         |          |          |           |               |                     |
| 691 | BR144  | Execute booking deletion | TC_BR144_005 | Verify associated invoice updated/cancelled           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.5.4: Search/Filter All Bookings

#### BR145 - Displaying Rules (Booking List)

| No  | Req ID | Req Desc                          | TC ID        | TC Desc                                                            | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------- | ------------ | ------------------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 692 | BR145  | Display booking list with filters | TC_BR145_001 | Verify all bookings loaded on screen entry                         |             |               |               |          |         |          |          |           |               |                     |
| 693 | BR145  | Display booking list with filters | TC_BR145_002 | Verify columns: Booking ID, Names, Event Date, Shift, Hall, Status |             |               |               |          |         |          |          |           |               |                     |
| 694 | BR145  | Display booking list with filters | TC_BR145_003 | Verify default sort by EventDate DESC                              |             |               |               |          |         |          |          |           |               |                     |
| 695 | BR145  | Display booking list with filters | TC_BR145_004 | Verify Total Amount column displayed                               |             |               |               |          |         |          |          |           |               |                     |

#### BR146 - Searching Rules (Filter Bookings)

| No  | Req ID | Req Desc                    | TC ID        | TC Desc                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ------------ | ----------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 696 | BR146  | Filter bookings by criteria | TC_BR146_001 | Verify filter by SearchText (name, phone) works |             |               |               |          |         |          |          |           |               |                     |
| 697 | BR146  | Filter bookings by criteria | TC_BR146_002 | Verify filter by DateRange (from-to) works      |             |               |               |          |         |          |          |           |               |                     |
| 698 | BR146  | Filter bookings by criteria | TC_BR146_003 | Verify filter by Status dropdown works          |             |               |               |          |         |          |          |           |               |                     |
| 699 | BR146  | Filter bookings by criteria | TC_BR146_004 | Verify filter by Shift dropdown works           |             |               |               |          |         |          |          |           |               |                     |
| 700 | BR146  | Filter bookings by criteria | TC_BR146_005 | Verify filter by Hall dropdown works            |             |               |               |          |         |          |          |           |               |                     |
| 701 | BR146  | Filter bookings by criteria | TC_BR146_006 | Verify MSG106 displays when no results found    |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.5.5: View Any Booking Details

#### BR147 - Displaying Rules (View Booking Details)

| No  | Req ID | Req Desc                    | TC ID        | TC Desc                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ------------ | ------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 702 | BR147  | Display any booking details | TC_BR147_001 | Verify full details display when booking selected      |             |               |               |          |         |          |          |           |               |                     |
| 703 | BR147  | Display any booking details | TC_BR147_002 | Verify customer info section: Groom/Bride names, phone |             |               |               |          |         |          |          |           |               |                     |
| 704 | BR147  | Display any booking details | TC_BR147_003 | Verify hall and event info section                     |             |               |               |          |         |          |          |           |               |                     |
| 705 | BR147  | Display any booking details | TC_BR147_004 | Verify menu items list with quantities and prices      |             |               |               |          |         |          |          |           |               |                     |
| 706 | BR147  | Display any booking details | TC_BR147_005 | Verify services list with prices                       |             |               |               |          |         |          |          |           |               |                     |

#### BR148 - Display Rules (Booking Actions)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | --------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 707 | BR148  | Display booking action buttons | TC_BR148_001 | Verify payment history section displays       |             |               |               |          |         |          |          |           |               |                     |
| 708 | BR148  | Display booking action buttons | TC_BR148_002 | Verify status timeline/history section        |             |               |               |          |         |          |          |           |               |                     |
| 709 | BR148  | Display booking action buttons | TC_BR148_003 | Verify Print button available                 |             |               |               |          |         |          |          |           |               |                     |
| 710 | BR148  | Display booking action buttons | TC_BR148_004 | Verify Export PDF button available            |             |               |               |          |         |          |          |           |               |                     |
| 711 | BR148  | Display booking action buttons | TC_BR148_005 | Verify Edit/Process Payment buttons available |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.5.6: Modify Booking Details

#### BR149 - Displaying Rules (Modify Booking)

| No  | Req ID | Req Desc                    | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------- | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 712 | BR149  | Display modify booking form | TC_BR149_001 | Verify MSG107 error when booking status = "Completed"               |             |               |               |          |         |          |          |           |               |                     |
| 713 | BR149  | Display modify booking form | TC_BR149_002 | Verify edit form displays with current values for editable bookings |             |               |               |          |         |          |          |           |               |                     |
| 714 | BR149  | Display modify booking form | TC_BR149_003 | Verify TableCount field is editable                                 |             |               |               |          |         |          |          |           |               |                     |
| 715 | BR149  | Display modify booking form | TC_BR149_004 | Verify Menu selection is editable                                   |             |               |               |          |         |          |          |           |               |                     |
| 716 | BR149  | Display modify booking form | TC_BR149_005 | Verify Services selection is editable                               |             |               |               |          |         |          |          |           |               |                     |

#### BR150 - Validation Rules (Modify Booking)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ---------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 717 | BR150  | Validate booking modifications | TC_BR150_001 | Verify validation similar to BR140             |             |               |               |          |         |          |          |           |               |                     |
| 718 | BR150  | Validate booking modifications | TC_BR150_002 | Verify MSG16 displays when no changes detected |             |               |               |          |         |          |          |           |               |                     |
| 719 | BR150  | Validate booking modifications | TC_BR150_003 | Verify TableCount cannot exceed hall capacity  |             |               |               |          |         |          |          |           |               |                     |
| 720 | BR150  | Validate booking modifications | TC_BR150_004 | Verify at least one menu item required         |             |               |               |          |         |          |          |           |               |                     |

#### BR151 - Processing Rules (Modify Booking)

| No  | Req ID | Req Desc                      | TC ID        | TC Desc                                        | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ------------ | ---------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 721 | BR151  | Execute booking modifications | TC_BR151_001 | Verify total amount recalculated after changes |             |               |               |          |         |          |          |           |               |                     |
| 722 | BR151  | Execute booking modifications | TC_BR151_002 | Verify Booking record updated in database      |             |               |               |          |         |          |          |           |               |                     |
| 723 | BR151  | Execute booking modifications | TC_BR151_003 | Verify associated invoice updated              |             |               |               |          |         |          |          |           |               |                     |
| 724 | BR151  | Execute booking modifications | TC_BR151_004 | Verify modification history logged             |             |               |               |          |         |          |          |           |               |                     |
| 725 | BR151  | Execute booking modifications | TC_BR151_005 | Verify MSG108 success notification displays    |             |               |               |          |         |          |          |           |               |                     |

---

## 6. Customer Payment & Invoice

### UC 2.1.6.1: View My Invoice & Debt

#### BR152 - Displaying Rules (Customer Invoice List)

| No  | Req ID | Req Desc                      | TC ID        | TC Desc                                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ------------ | ---------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 726 | BR152  | Display customer invoice list | TC_BR152_001 | Verify invoice page displays for logged-in customer                    |             |               |               |          |         |          |          |           |               |                     |
| 727 | BR152  | Display customer invoice list | TC_BR152_002 | Verify query filters by current customer ID                            |             |               |               |          |         |          |          |           |               |                     |
| 728 | BR152  | Display customer invoice list | TC_BR152_003 | Verify columns: Booking ID, Event Date, Total, Paid, Remaining, Status |             |               |               |          |         |          |          |           |               |                     |
| 729 | BR152  | Display customer invoice list | TC_BR152_004 | Verify outstanding debt highlighted                                    |             |               |               |          |         |          |          |           |               |                     |

#### BR153 - Selection Rules (View Invoice Details)

| No  | Req ID | Req Desc                         | TC ID        | TC Desc                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ------------ | ----------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 730 | BR153  | Display selected invoice details | TC_BR153_001 | Verify detailed breakdown when invoice selected |             |               |               |          |         |          |          |           |               |                     |
| 731 | BR153  | Display selected invoice details | TC_BR153_002 | Verify hall cost displayed                      |             |               |               |          |         |          |          |           |               |                     |
| 732 | BR153  | Display selected invoice details | TC_BR153_003 | Verify menu cost displayed                      |             |               |               |          |         |          |          |           |               |                     |
| 733 | BR153  | Display selected invoice details | TC_BR153_004 | Verify services cost displayed                  |             |               |               |          |         |          |          |           |               |                     |
| 734 | BR153  | Display selected invoice details | TC_BR153_005 | Verify deposit paid amount displayed            |             |               |               |          |         |          |          |           |               |                     |
| 735 | BR153  | Display selected invoice details | TC_BR153_006 | Verify remaining balance displayed              |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.6.2: Pay My Invoice

#### BR154 - Displaying Rules (Customer Payment Form)

| No  | Req ID | Req Desc                      | TC ID        | TC Desc                                                 | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ------------ | ------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 736 | BR154  | Display customer payment form | TC_BR154_001 | Verify payment form displays when clicking "Thanh toán" |             |               |               |          |         |          |          |           |               |                     |
| 737 | BR154  | Display customer payment form | TC_BR154_002 | Verify PaymentAmount default = remaining balance        |             |               |               |          |         |          |          |           |               |                     |
| 738 | BR154  | Display customer payment form | TC_BR154_003 | Verify PaymentMethod options displayed                  |             |               |               |          |         |          |          |           |               |                     |
| 739 | BR154  | Display customer payment form | TC_BR154_004 | Verify minimum deposit requirement displayed            |             |               |               |          |         |          |          |           |               |                     |

#### BR155 - Validation Rules (Customer Payment)

| No  | Req ID | Req Desc                       | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------ | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 740 | BR155  | Validate customer payment data | TC_BR155_001 | Verify MSG109 when PaymentAmount < MinDepositAmount (first payment) |             |               |               |          |         |          |          |           |               |                     |
| 741 | BR155  | Validate customer payment data | TC_BR155_002 | Verify MSG110 when PaymentAmount > RemainingBalance                 |             |               |               |          |         |          |          |           |               |                     |
| 742 | BR155  | Validate customer payment data | TC_BR155_003 | Verify payment method is selected                                   |             |               |               |          |         |          |          |           |               |                     |
| 743 | BR155  | Validate customer payment data | TC_BR155_004 | Verify validation passes with valid amount and method               |             |               |               |          |         |          |          |           |               |                     |

#### BR156 - Processing Rules (Customer Payment)

| No  | Req ID | Req Desc                 | TC ID        | TC Desc                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------ | ------------ | ------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 744 | BR156  | Process customer payment | TC_BR156_001 | Verify payment record created                          |             |               |               |          |         |          |          |           |               |                     |
| 745 | BR156  | Process customer payment | TC_BR156_002 | Verify invoice paid amount updated                     |             |               |               |          |         |          |          |           |               |                     |
| 746 | BR156  | Process customer payment | TC_BR156_003 | Verify remaining balance recalculated                  |             |               |               |          |         |          |          |           |               |                     |
| 747 | BR156  | Process customer payment | TC_BR156_004 | Verify invoice status = "Paid" when remaining = 0      |             |               |               |          |         |          |          |           |               |                     |
| 748 | BR156  | Process customer payment | TC_BR156_005 | Verify booking status = "Confirmed" after full payment |             |               |               |          |         |          |          |           |               |                     |
| 749 | BR156  | Process customer payment | TC_BR156_006 | Verify MSG111 success notification displays            |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.6.3: Export My Invoice to PDF

#### BR157 - Displaying Rules (Customer Export PDF)

| No  | Req ID | Req Desc                  | TC ID        | TC Desc                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------- | ------------ | --------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 750 | BR157  | Display PDF export option | TC_BR157_001 | Verify "Xuất PDF" button visible on invoice details |             |               |               |          |         |          |          |           |               |                     |
| 751 | BR157  | Display PDF export option | TC_BR157_002 | Verify invoice data prepared for export             |             |               |               |          |         |          |          |           |               |                     |
| 752 | BR157  | Display PDF export option | TC_BR157_003 | Verify company logo included in export data         |             |               |               |          |         |          |          |           |               |                     |

#### BR158 - Processing Rules (Customer Export PDF)

| No  | Req ID | Req Desc                      | TC ID        | TC Desc                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------- | ------------ | ---------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 753 | BR158  | Generate customer invoice PDF | TC_BR158_001 | Verify PDF generated using iText library                   |             |               |               |          |         |          |          |           |               |                     |
| 754 | BR158  | Generate customer invoice PDF | TC_BR158_002 | Verify PDF includes header with company info               |             |               |               |          |         |          |          |           |               |                     |
| 755 | BR158  | Generate customer invoice PDF | TC_BR158_003 | Verify PDF includes invoice number and customer details    |             |               |               |          |         |          |          |           |               |                     |
| 756 | BR158  | Generate customer invoice PDF | TC_BR158_004 | Verify PDF includes booking details and cost breakdown     |             |               |               |          |         |          |          |           |               |                     |
| 757 | BR158  | Generate customer invoice PDF | TC_BR158_005 | Verify PDF includes payment summary and terms              |             |               |               |          |         |          |          |           |               |                     |
| 758 | BR158  | Generate customer invoice PDF | TC_BR158_006 | Verify filename format "HoaDon*[InvoiceId]*[yyyyMMdd].pdf" |             |               |               |          |         |          |          |           |               |                     |
| 759 | BR158  | Generate customer invoice PDF | TC_BR158_007 | Verify MSG112 success notification displays                |             |               |               |          |         |          |          |           |               |                     |

---

## 7. Staff Invoice Management

### UC 2.1.7.1: View Any Invoice & Debt

#### BR159 - Displaying Rules (Staff View Invoice)

| No  | Req ID | Req Desc                             | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------ | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 760 | BR159  | Display invoice from booking details | TC_BR159_001 | Verify "Xem hóa đơn" button on Booking Details (UC 2.1.5.5)         |             |               |               |          |         |          |          |           |               |                     |
| 761 | BR159  | Display invoice from booking details | TC_BR159_002 | Verify query joins Booking, Menu, ServiceDetail, Hall, Shift tables |             |               |               |          |         |          |          |           |               |                     |
| 762 | BR159  | Display invoice from booking details | TC_BR159_003 | Verify InvoiceView displays all cost components                     |             |               |               |          |         |          |          |           |               |                     |
| 763 | BR159  | Display invoice from booking details | TC_BR159_004 | Verify Deposit amount displayed                                     |             |               |               |          |         |          |          |           |               |                     |
| 764 | BR159  | Display invoice from booking details | TC_BR159_005 | Verify Fine/Penalty amount displayed if applicable                  |             |               |               |          |         |          |          |           |               |                     |

#### BR160 - Display Rules (Invoice Actions)

| No  | Req ID | Req Desc                           | TC ID        | TC Desc                                                  | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------- | ------------ | -------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 765 | BR160  | Display invoice status and actions | TC_BR160_001 | Verify payment status displayed (Paid/Unpaid)            |             |               |               |          |         |          |          |           |               |                     |
| 766 | BR160  | Display invoice status and actions | TC_BR160_002 | Verify "Confirm Payment" button when RemainingAmount > 0 |             |               |               |          |         |          |          |           |               |                     |
| 767 | BR160  | Display invoice status and actions | TC_BR160_003 | Verify "Export PDF" button available                     |             |               |               |          |         |          |          |           |               |                     |
| 768 | BR160  | Display invoice status and actions | TC_BR160_004 | Verify remaining debt highlighted with color coding      |             |               |               |          |         |          |          |           |               |                     |
| 769 | BR160  | Display invoice status and actions | TC_BR160_005 | Verify buttons hidden when invoice fully paid            |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.7.2 - Confirm Payment & Calculate Penalty (Xác nhận thanh toán và tính tiền phạt)

#### BR161 - Payment Confirmation Rules

| No  | Req ID | Req Desc                                   | TC ID        | TC Desc                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ------------------------------------------ | ------------ | ---------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 770 | BR161  | Allow staff to confirm payment for invoice | TC_BR161_001 | Verify staff can access payment confirmation from invoice detail |             |               |               |          |         |          |          |           |               |                     |
| 771 | BR161  | Allow staff to confirm payment for invoice | TC_BR161_002 | Verify payment amount input field accepts valid amount           |             |               |               |          |         |          |          |           |               |                     |
| 772 | BR161  | Allow staff to confirm payment for invoice | TC_BR161_003 | Verify payment cannot exceed remaining debt                      |             |               |               |          |         |          |          |           |               |                     |
| 773 | BR161  | Allow staff to confirm payment for invoice | TC_BR161_004 | Verify payment date automatically set to current date            |             |               |               |          |         |          |          |           |               |                     |
| 774 | BR161  | Allow staff to confirm payment for invoice | TC_BR161_005 | Verify error when payment amount is zero or negative             |             |               |               |          |         |          |          |           |               |                     |

#### BR162 - Penalty Calculation Rules

| No  | Req ID | Req Desc                                      | TC ID        | TC Desc                                                             | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------------- | ------------ | ------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 775 | BR162  | Calculate penalty based on late payment rules | TC_BR162_001 | Verify penalty calculated when payment after WeddingDate            |             |               |               |          |         |          |          |           |               |                     |
| 776 | BR162  | Calculate penalty based on late payment rules | TC_BR162_002 | Verify penalty percentage from system parameter TI_LE_PHAT_TRA_CHAM |             |               |               |          |         |          |          |           |               |                     |
| 777 | BR162  | Calculate penalty based on late payment rules | TC_BR162_003 | Verify penalty = RemainingAmount × PenaltyRate                      |             |               |               |          |         |          |          |           |               |                     |
| 778 | BR162  | Calculate penalty based on late payment rules | TC_BR162_004 | Verify no penalty when payment before or on WeddingDate             |             |               |               |          |         |          |          |           |               |                     |
| 779 | BR162  | Calculate penalty based on late payment rules | TC_BR162_005 | Verify penalty displayed to staff before confirmation               |             |               |               |          |         |          |          |           |               |                     |

#### BR163 - Payment Update Rules

| No  | Req ID | Req Desc                                  | TC ID        | TC Desc                                                          | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------------- | ------------ | ---------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 780 | BR163  | Update invoice after payment confirmation | TC_BR163_001 | Verify TotalPaid updated after payment confirmation              |             |               |               |          |         |          |          |           |               |                     |
| 781 | BR163  | Update invoice after payment confirmation | TC_BR163_002 | Verify RemainingAmount recalculated correctly                    |             |               |               |          |         |          |          |           |               |                     |
| 782 | BR163  | Update invoice after payment confirmation | TC_BR163_003 | Verify PaymentDate recorded in database                          |             |               |               |          |         |          |          |           |               |                     |
| 783 | BR163  | Update invoice after payment confirmation | TC_BR163_004 | Verify invoice status changes to "Paid" when RemainingAmount = 0 |             |               |               |          |         |          |          |           |               |                     |
| 784 | BR163  | Update invoice after payment confirmation | TC_BR163_005 | Verify success message displayed after payment confirmed         |             |               |               |          |         |          |          |           |               |                     |

#### BR164 - Penalty Recording Rules

| No  | Req ID | Req Desc                         | TC ID        | TC Desc                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | -------------------------------- | ------------ | --------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 785 | BR164  | Record penalty amount in invoice | TC_BR164_001 | Verify PenaltyAmount field updated in database            |             |               |               |          |         |          |          |           |               |                     |
| 786 | BR164  | Record penalty amount in invoice | TC_BR164_002 | Verify penalty added to TotalAmount for final calculation |             |               |               |          |         |          |          |           |               |                     |
| 787 | BR164  | Record penalty amount in invoice | TC_BR164_003 | Verify penalty visible on invoice detail view             |             |               |               |          |         |          |          |           |               |                     |
| 788 | BR164  | Record penalty amount in invoice | TC_BR164_004 | Verify penalty included in exported PDF                   |             |               |               |          |         |          |          |           |               |                     |
| 789 | BR164  | Record penalty amount in invoice | TC_BR164_005 | Verify penalty calculation audit trail maintained         |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.7.3 - Export Any Invoice to PDF (Xuất hóa đơn bất kỳ ra file PDF)

#### BR165 - Staff Export Invoice PDF Access Rules

| No  | Req ID | Req Desc                                 | TC ID        | TC Desc                                                | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ---------------------------------------- | ------------ | ------------------------------------------------------ | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 790 | BR165  | Allow staff to export any invoice to PDF | TC_BR165_001 | Verify staff can access export PDF from invoice list   |             |               |               |          |         |          |          |           |               |                     |
| 791 | BR165  | Allow staff to export any invoice to PDF | TC_BR165_002 | Verify staff can export PDF from invoice detail view   |             |               |               |          |         |          |          |           |               |                     |
| 792 | BR165  | Allow staff to export any invoice to PDF | TC_BR165_003 | Verify staff without permission cannot export          |             |               |               |          |         |          |          |           |               |                     |
| 793 | BR165  | Allow staff to export any invoice to PDF | TC_BR165_004 | Verify export button visible only for authorized users |             |               |               |          |         |          |          |           |               |                     |
| 794 | BR165  | Allow staff to export any invoice to PDF | TC_BR165_005 | Verify PDF export for both paid and unpaid invoices    |             |               |               |          |         |          |          |           |               |                     |

#### BR166 - Staff Invoice PDF Content Rules

| No  | Req ID | Req Desc                                  | TC ID        | TC Desc                                                                    | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------------- | ------------ | -------------------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 795 | BR166  | PDF contains complete invoice information | TC_BR166_001 | Verify PDF includes customer information (name, phone, address)            |             |               |               |          |         |          |          |           |               |                     |
| 796 | BR166  | PDF contains complete invoice information | TC_BR166_002 | Verify PDF includes wedding details (date, shift, hall)                    |             |               |               |          |         |          |          |           |               |                     |
| 797 | BR166  | PDF contains complete invoice information | TC_BR166_003 | Verify PDF includes itemized menu and services                             |             |               |               |          |         |          |          |           |               |                     |
| 798 | BR166  | PDF contains complete invoice information | TC_BR166_004 | Verify PDF includes all amounts (deposit, total, paid, remaining, penalty) |             |               |               |          |         |          |          |           |               |                     |
| 799 | BR166  | PDF contains complete invoice information | TC_BR166_005 | Verify PDF filename includes invoice ID and date                           |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.8.1 - View Revenue Report (Xem báo cáo doanh thu)

#### BR167 - Revenue Report Access Rules

| No  | Req ID | Req Desc                                      | TC ID        | TC Desc                                                       | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------------- | ------------ | ------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 800 | BR167  | Allow authorized users to view revenue report | TC_BR167_001 | Verify admin can access revenue report menu                   |             |               |               |          |         |          |          |           |               |                     |
| 801 | BR167  | Allow authorized users to view revenue report | TC_BR167_002 | Verify staff with permission can access revenue report        |             |               |               |          |         |          |          |           |               |                     |
| 802 | BR167  | Allow authorized users to view revenue report | TC_BR167_003 | Verify unauthorized users cannot access revenue report        |             |               |               |          |         |          |          |           |               |                     |
| 803 | BR167  | Allow authorized users to view revenue report | TC_BR167_004 | Verify revenue report menu item hidden for unauthorized users |             |               |               |          |         |          |          |           |               |                     |
| 804 | BR167  | Allow authorized users to view revenue report | TC_BR167_005 | Verify permission check logged for audit                      |             |               |               |          |         |          |          |           |               |                     |

#### BR168 - Revenue Report Filter Rules

| No  | Req ID | Req Desc                                | TC ID        | TC Desc                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------- | ------------ | --------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 805 | BR168  | Filter revenue report by month and year | TC_BR168_001 | Verify month selector dropdown displays all months (1-12) |             |               |               |          |         |          |          |           |               |                     |
| 806 | BR168  | Filter revenue report by month and year | TC_BR168_002 | Verify year selector shows available years with data      |             |               |               |          |         |          |          |           |               |                     |
| 807 | BR168  | Filter revenue report by month and year | TC_BR168_003 | Verify report refreshes when filter changes               |             |               |               |          |         |          |          |           |               |                     |
| 808 | BR168  | Filter revenue report by month and year | TC_BR168_004 | Verify default filter shows current month/year            |             |               |               |          |         |          |          |           |               |                     |
| 809 | BR168  | Filter revenue report by month and year | TC_BR168_005 | Verify empty state when no data for selected period       |             |               |               |          |         |          |          |           |               |                     |

#### BR169 - Revenue Report Display Rules

| No  | Req ID | Req Desc                                  | TC ID        | TC Desc                                                         | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------------- | ------------ | --------------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 810 | BR169  | Display revenue grouped by day with chart | TC_BR169_001 | Verify daily revenue displayed in table format                  |             |               |               |          |         |          |          |           |               |                     |
| 811 | BR169  | Display revenue grouped by day with chart | TC_BR169_002 | Verify revenue chart (bar/line) renders correctly               |             |               |               |          |         |          |          |           |               |                     |
| 812 | BR169  | Display revenue grouped by day with chart | TC_BR169_003 | Verify total monthly revenue calculated and displayed           |             |               |               |          |         |          |          |           |               |                     |
| 813 | BR169  | Display revenue grouped by day with chart | TC_BR169_004 | Verify revenue includes completed weddings only (paid invoices) |             |               |               |          |         |          |          |           |               |                     |
| 814 | BR169  | Display revenue grouped by day with chart | TC_BR169_005 | Verify percentage/ratio calculation for each day vs total       |             |               |               |          |         |          |          |           |               |                     |

---

### UC 2.1.8.2 - Export Report to Excel (Xuất báo cáo ra file Excel)

#### BR170 - Export Report Access Rules

| No  | Req ID | Req Desc                                | TC ID        | TC Desc                                                     | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | --------------------------------------- | ------------ | ----------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 815 | BR170  | Allow exporting revenue report to Excel | TC_BR170_001 | Verify export button visible on revenue report screen       |             |               |               |          |         |          |          |           |               |                     |
| 816 | BR170  | Allow exporting revenue report to Excel | TC_BR170_002 | Verify export triggers file save dialog                     |             |               |               |          |         |          |          |           |               |                     |
| 817 | BR170  | Allow exporting revenue report to Excel | TC_BR170_003 | Verify default filename includes report period (month-year) |             |               |               |          |         |          |          |           |               |                     |
| 818 | BR170  | Allow exporting revenue report to Excel | TC_BR170_004 | Verify export only available when report has data           |             |               |               |          |         |          |          |           |               |                     |
| 819 | BR170  | Allow exporting revenue report to Excel | TC_BR170_005 | Verify export permission checked before allowing            |             |               |               |          |         |          |          |           |               |                     |

#### BR171 - Excel Report Content Rules

| No  | Req ID | Req Desc                            | TC ID        | TC Desc                                                   | Test Design | Test Designer | UAT Test Req? | Test Env | UAT Env | Prod Env | Defects? | Defect ID | Defect Status | Req Coverage Status |
| --- | ------ | ----------------------------------- | ------------ | --------------------------------------------------------- | ----------- | ------------- | ------------- | -------- | ------- | -------- | -------- | --------- | ------------- | ------------------- |
| 820 | BR171  | Excel contains complete report data | TC_BR171_001 | Verify Excel includes report header with period info      |             |               |               |          |         |          |          |           |               |                     |
| 821 | BR171  | Excel contains complete report data | TC_BR171_002 | Verify Excel includes daily revenue data rows             |             |               |               |          |         |          |          |           |               |                     |
| 822 | BR171  | Excel contains complete report data | TC_BR171_003 | Verify Excel includes total/summary row                   |             |               |               |          |         |          |          |           |               |                     |
| 823 | BR171  | Excel contains complete report data | TC_BR171_004 | Verify Excel formatting (currency, dates, alignment)      |             |               |               |          |         |          |          |           |               |                     |
| 824 | BR171  | Excel contains complete report data | TC_BR171_005 | Verify Excel file opens successfully in Excel/LibreOffice |             |               |               |          |         |          |          |           |               |                     |

---

## Test Case Summary

| BR Code   | Use Case                   | Total Test Cases |
| --------- | -------------------------- | ---------------- |
| BR1       | Login                      | 5                |
| BR2       | Login                      | 5                |
| BR3       | Login                      | 8                |
| BR4       | Logout                     | 6                |
| BR5       | Manage Profile             | 5                |
| BR6       | Manage Profile             | 6                |
| BR7       | Manage Profile             | 5                |
| BR8       | Change Password            | 5                |
| BR9       | Change Password            | 5                |
| BR10      | Change Password            | 5                |
| BR11      | View User Details          | 5                |
| BR12      | View User Details          | 5                |
| BR13      | View User Details          | 5                |
| BR14      | Add New User               | 5                |
| BR15      | Add New User               | 6                |
| BR16      | Add New User               | 5                |
| BR17      | Edit User                  | 5                |
| BR18      | Edit User                  | 7                |
| BR19      | Edit User                  | 5                |
| BR20      | Delete User                | 3                |
| BR21      | Delete User                | 3                |
| BR22      | Delete User                | 3                |
| BR23      | Delete User                | 5                |
| BR24      | View Permission Group      | 5                |
| BR25      | View Permission Group      | 3                |
| BR26      | View Permission Group      | 5                |
| BR27      | Add New Permission Group   | 4                |
| BR28      | Add New Permission Group   | 5                |
| BR29      | Add New Permission Group   | 5                |
| BR30      | Edit Permission Group      | 5                |
| BR31      | Edit Permission Group      | 5                |
| BR32      | Edit Permission Group      | 5                |
| BR33      | Delete Permission Group    | 3                |
| BR34      | Delete Permission Group    | 4                |
| BR35      | Delete Permission Group    | 3                |
| BR36      | Delete Permission Group    | 5                |
| BR37      | Manage System Parameters   | 6                |
| BR38      | Manage System Parameters   | 9                |
| BR39      | Manage System Parameters   | 6                |
| BR40      | Manage System Parameters   | 5                |
| BR41      | View Hall Details          | 5                |
| BR42      | View Hall Details          | 5                |
| BR43      | View Hall Details          | 5                |
| BR44      | Add New Hall               | 5                |
| BR45      | Add New Hall               | 6                |
| BR46      | Add New Hall               | 5                |
| BR47      | Edit Hall                  | 5                |
| BR48      | Edit Hall                  | 6                |
| BR49      | Edit Hall                  | 5                |
| BR50      | Delete Hall                | 4                |
| BR51      | Delete Hall                | 4                |
| BR52      | Delete Hall                | 3                |
| BR53      | Delete Hall                | 5                |
| BR54      | Export Halls to Excel      | 4                |
| BR55      | Export Halls to Excel      | 4                |
| BR56      | Export Halls to Excel      | 7                |
| BR57      | View Hall Type Details     | 5                |
| BR58      | View Hall Type Details     | 4                |
| BR59      | View Hall Type Details     | 4                |
| BR60      | Add New Hall Type          | 4                |
| BR61      | Add New Hall Type          | 5                |
| BR62      | Add New Hall Type          | 5                |
| BR63      | Edit Hall Type             | 4                |
| BR64      | Edit Hall Type             | 5                |
| BR65      | Edit Hall Type             | 5                |
| BR66      | Delete Hall Type           | 3                |
| BR67      | Delete Hall Type           | 4                |
| BR68      | Delete Hall Type           | 3                |
| BR69      | Delete Hall Type           | 5                |
| BR70      | Export Hall Types to Excel | 4                |
| BR71      | Export Hall Types to Excel | 4                |
| BR72      | Export Hall Types to Excel | 7                |
| BR73      | View Dish Details          | 5                |
| BR74      | View Dish Details          | 5                |
| BR75      | View Dish Details          | 5                |
| BR76      | Add New Dish               | 4                |
| BR77      | Add New Dish               | 6                |
| BR78      | Add New Dish               | 6                |
| BR79      | Edit Dish                  | 4                |
| BR80      | Edit Dish                  | 6                |
| BR81      | Edit Dish                  | 6                |
| BR82      | Delete Dish                | 4                |
| BR83      | Delete Dish                | 4                |
| BR84      | Delete Dish                | 3                |
| BR85      | Delete Dish                | 5                |
| BR86      | Export Dishes to Excel     | 4                |
| BR87      | Export Dishes to Excel     | 4                |
| BR88      | Export Dishes to Excel     | 7                |
| BR89      | View Service Details       | 5                |
| BR90      | View Service Details       | 5                |
| BR91      | View Service Details       | 5                |
| BR92      | Add New Service            | 4                |
| BR93      | Add New Service            | 5                |
| BR94      | Add New Service            | 6                |
| BR95      | Edit Service               | 4                |
| BR96      | Edit Service               | 6                |
| BR97      | Edit Service               | 6                |
| BR98      | Delete Service             | 4                |
| BR99      | Delete Service             | 4                |
| BR100     | Delete Service             | 3                |
| BR101     | Delete Service             | 5                |
| BR102     | Export Services to Excel   | 4                |
| BR103     | Export Services to Excel   | 4                |
| BR104     | Export Services to Excel   | 7                |
| BR105     | View Shift Details         | 5                |
| BR106     | View Shift Details         | 4                |
| BR107     | View Shift Details         | 5                |
| BR108     | View Shift Details         | 4                |
| BR109     | Add New Shift              | 4                |
| BR110     | Add New Shift              | 5                |
| BR111     | Add New Shift              | 5                |
| BR112     | Edit Shift                 | 4                |
| BR113     | Edit Shift                 | 5                |
| BR114     | Delete Shift               | 4                |
| BR115     | Delete Shift               | 5                |
| BR116     | Delete Shift               | 4                |
| BR117     | Delete Shift               | 5                |
| BR118     | Export Shifts to Excel     | 4                |
| BR119     | Export Shifts to Excel     | 4                |
| BR120     | Export Shifts to Excel     | 7                |
| BR121     | Check Hall Availability    | 5                |
| BR122     | Check Hall Availability    | 4                |
| BR123     | Check Hall Availability    | 5                |
| BR124     | Submit Wedding Reservation | 5                |
| BR125     | Submit Wedding Reservation | 6                |
| BR126     | Submit Wedding Reservation | 5                |
| BR127     | View My Booking Details    | 5                |
| BR128     | View My Booking Details    | 6                |
| BR129     | Edit My Booking Request    | 4                |
| BR130     | Edit My Booking Request    | 5                |
| BR131     | Edit My Booking Request    | 5                |
| BR132     | Cancel My Booking          | 4                |
| BR133     | Cancel My Booking          | 4                |
| BR134     | Cancel My Booking          | 5                |
| BR135     | Cancel My Booking          | 4                |
| BR136     | Cancel My Booking          | 5                |
| BR137     | Check System Hall Avail.   | 4                |
| BR138     | Check System Hall Avail.   | 5                |
| BR139     | Create Booking for Cust.   | 5                |
| BR140     | Create Booking for Cust.   | 5                |
| BR141     | Create Booking for Cust.   | 5                |
| BR142     | Delete Booking             | 3                |
| BR143     | Delete Booking             | 4                |
| BR144     | Delete Booking             | 5                |
| BR145     | Search/Filter Bookings     | 4                |
| BR146     | Search/Filter Bookings     | 6                |
| BR147     | View Any Booking Details   | 5                |
| BR148     | View Any Booking Details   | 5                |
| BR149     | Modify Booking Details     | 5                |
| BR150     | Modify Booking Details     | 4                |
| BR151     | Modify Booking Details     | 5                |
| BR152     | View My Invoice & Debt     | 4                |
| BR153     | View My Invoice & Debt     | 6                |
| BR154     | Pay My Invoice             | 4                |
| BR155     | Pay My Invoice             | 4                |
| BR156     | Pay My Invoice             | 6                |
| BR157     | Export Invoice to PDF      | 3                |
| BR158     | Export Invoice to PDF      | 7                |
| BR159     | View Any Invoice & Debt    | 5                |
| BR160     | View Any Invoice & Debt    | 5                |
| BR161     | Confirm Payment            | 5                |
| BR162     | Penalty Calculation        | 5                |
| BR163     | Payment Update             | 5                |
| BR164     | Penalty Recording          | 5                |
| BR165     | Export Any Invoice PDF     | 5                |
| BR166     | Invoice PDF Content        | 5                |
| BR167     | Revenue Report Access      | 5                |
| BR168     | Revenue Report Filter      | 5                |
| BR169     | Revenue Report Display     | 5                |
| BR170     | Export Report Access       | 5                |
| BR171     | Excel Report Content       | 5                |
| **Total** |                            | **824**          |

---

## Legend

| Column              | Description                                                 |
| ------------------- | ----------------------------------------------------------- |
| No                  | Sequential number of test case                              |
| Req ID              | Business Rule ID from SRS                                   |
| Req Desc            | Brief description of the requirement                        |
| TC ID               | Unique Test Case identifier (TC*BR{X}*{NNN})                |
| TC Desc             | Detailed description of test case                           |
| Test Design         | Status: Completed / In Progress / Not Started               |
| Test Designer       | Name of person who designed the test case                   |
| UAT Test Req?       | Yes / No - Is UAT testing required?                         |
| Test Env            | Test environment execution status: Passed / Failed / No Run |
| UAT Env             | UAT environment execution status: Passed / Failed / No Run  |
| Prod Env            | Production environment status: Run / No Run                 |
| Defects?            | None / Yes - Are there defects found?                       |
| Defect ID           | Defect tracking ID if defects found                         |
| Defect Status       | Open / In Progress / Fixed / Closed / N/A                   |
| Req Coverage Status | Full / Partial / None - Coverage status of requirement      |

---

**Document Version**: 1.6  
**Created Date**: 03/12/2025  
**Last Updated**: 03/12/2025  
**Based on**: WMS_SRS_v1.8.0_final.md  
**Total BRs covered**: 171 / 171 ✅
