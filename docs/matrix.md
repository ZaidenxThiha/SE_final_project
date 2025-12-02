# Test Traceability Matrix (Core Logic, EP/BVA)

| Requirement | Description | Test Case ID(s) | Technique | Status |
| --- | --- | --- | --- | --- |
| R1 | Order totals use current product price; stock decremented on paid-like statuses; stock restored on cancel if previously adjusted | O1 CreateOrder_RepricesItemsAndSetsTotal; O3 UpdateStatus_DecrementsStockOnce_ForPaidLikeStatuses; O4 UpdateStatus_Cancelled_RestoresStock_WhenPreviouslyAdjusted | EP/BVA | Passed |
| R2 | Product name required; price > 0; must exist to update | P1 Create_InvalidName_Throws; P2 Create_PriceZero_Throws; P3 Update_NotFound_Throws; P4 Update_Valid_Succeeds | EP/BVA | Passed |
| R3 | Auth uses hashed passwords; admin fallback only when no hash; change password requires current and updates hash | U1 Login_WithHashedPassword_Succeeds; U2 Login_AdminFallback_OnlyWhenNoHashAndPasswordMatches; U3 Login_WrongPassword_Fails; U4 ChangePassword_BadCurrent_Throws; U5 ChangePassword_Valid_UpdatesHash | EP/BVA | Passed |
| R4 | Order creation requires at least one item | O2 CreateOrder_NoItems_Throws | EP | Passed |

Legend: EP = Equivalence Partitioning; BVA = Boundary Value Analysis. Status reflects latest `dotnet test` run.***
