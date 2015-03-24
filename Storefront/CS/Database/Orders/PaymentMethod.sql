USE [$(CS_DB_NAME_ORDERS_CONFIG)]
GO
-----------------------------------------------------
-- Table name: PaymentMethod
-----------------------------------------------------
DELETE FROM [PaymentMethod]

INSERT [dbo].[PaymentMethod] ([PaymentMethodId], [LanguageId], [PaymentMethodName], [Description], [PaymentProcessor], [ConfiguredMode], [PaymentType], [Enabled], [GroupId], [Created], [LastModified], [IsDefault]) VALUES (N'89283b19-b997-4461-bf77-81da7a1bc128', N'en-US', N'Visa', N'Visa', N'', 0, 1, 1, N'eacd9906-5fb0-44ea-9050-c770df59f17c', CAST(0x0000A11900F247DF AS DateTime), CAST(0x0000A119010E0D07 AS DateTime), 1)
INSERT [dbo].[PaymentMethod] ([PaymentMethodId], [LanguageId], [PaymentMethodName], [Description], [PaymentProcessor], [ConfiguredMode], [PaymentType], [Enabled], [GroupId], [Created], [LastModified], [IsDefault]) VALUES (N'534be5db-84f8-4c8c-8852-a181f56ad26e', N'en-US', N'Mastercard', N'Mastercard', N'', 0, 1, 1, N'984961dc-e8e5-4a2f-b264-9859ac4e7226', CAST(0x0000A11900F247DF AS DateTime), CAST(0x0000A119010E0D07 AS DateTime), 1)
INSERT [dbo].[PaymentMethod] ([PaymentMethodId], [LanguageId], [PaymentMethodName], [Description], [PaymentProcessor], [ConfiguredMode], [PaymentType], [Enabled], [GroupId], [Created], [LastModified], [IsDefault]) VALUES (N'711c532d-8313-4316-83f1-98e92e67c569', N'en-US', N'Amex', N'Amex', N'', 0, 1, 1, N'526642f9-51c2-40a9-ac49-7509d484baa0', CAST(0x0000A11900F247DF AS DateTime), CAST(0x0000A119010E0D07 AS DateTime), 1)
INSERT [dbo].[PaymentMethod] ([PaymentMethodId], [LanguageId], [PaymentMethodName], [Description], [PaymentProcessor], [ConfiguredMode], [PaymentType], [Enabled], [GroupId], [Created], [LastModified], [IsDefault]) VALUES (N'e6187c02-4daf-4d30-9ccf-d41ebb9cdeeb', N'en-US', N'Discover', N'Discover', N'', 0, 1, 1, N'3b9e95d1-8a3a-48dd-b419-ba8afd41c21d', CAST(0x0000A11900F247DF AS DateTime), CAST(0x0000A119010E0D07 AS DateTime), 1)

INSERT [dbo].[PaymentMethod] ([PaymentMethodId], [LanguageId], [PaymentMethodName], [Description], [PaymentProcessor], [ConfiguredMode], [PaymentType], [Enabled], [GroupId], [Created], [LastModified], [IsDefault]) VALUES (N'6266bf3a-831c-4c06-815b-4d840890ef3d', N'en-US', N'GiftCard', N'Gift Card', N'', 0, 4, 1, N'25422753-2f89-4287-afcb-38cac0b97504', CAST(0x0000A11900F247DF AS DateTime), CAST(0x0000A119010E0D07 AS DateTime), 1)

GO


