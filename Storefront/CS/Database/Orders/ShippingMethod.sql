USE [$(CS_DB_NAME_ORDERS_CONFIG)]
GO
-----------------------------------------------------
-- Table name: ShippingMethod
-----------------------------------------------------
DELETE FROM [ShippingMethod]
DELETE FROM [ShippingRates]

INSERT [dbo].[ShippingMethod] ([ShippingMethodId], [LanguageId], [ShippingMethodName], [Description], [ShippingCostCalculator], [ConfiguredMode], [Enabled], [GroupId], [IsDefault], [Created], [LastModified]) VALUES (N'3a369ab3-9e25-4fd6-bc11-6576b6c12394', N'en-US', N'Standard', N'Standard', N'commerce.stepwiseshipping', 0, 1, N'c0053cda-10bd-4329-8f24-edef93b92a88', 1, CAST(0x0000A11901138649 AS DateTime), CAST(0x0000A1190113B409 AS DateTime))
INSERT [dbo].[ShippingMethod] ([ShippingMethodId], [LanguageId], [ShippingMethodName], [Description], [ShippingCostCalculator], [ConfiguredMode], [Enabled], [GroupId], [IsDefault], [Created], [LastModified]) VALUES (N'9ee46420-45e1-4b5e-bb5a-9a3c2fb729dc', N'en-US', N'Next day air', N'Next day air', N'commerce.stepwiseshipping', 0, 1, N'1bed28d1-e239-4432-8f53-dd90756b6d1d', 1, CAST(0x0000A11901138649 AS DateTime), CAST(0x0000A1190113B409 AS DateTime))
INSERT [dbo].[ShippingMethod] ([ShippingMethodId], [LanguageId], [ShippingMethodName], [Description], [ShippingCostCalculator], [ConfiguredMode], [Enabled], [GroupId], [IsDefault], [Created], [LastModified]) VALUES (N'4e60faf7-92d9-451c-bbc6-3a2b9c182627', N'en-US', N'Standard overnight', N'Standard overnight', N'commerce.stepwiseshipping', 0, 1, N'00d38097-eea6-41b3-81df-d27a030cb816', 1, CAST(0x0000A11901138649 AS DateTime), CAST(0x0000A1190113B409 AS DateTime))
INSERT [dbo].[ShippingMethod] ([ShippingMethodId], [LanguageId], [ShippingMethodName], [Description], [ShippingCostCalculator], [ConfiguredMode], [Enabled], [GroupId], [IsDefault], [Created], [LastModified]) VALUES (N'78c63768-f925-4775-8818-4da4ff527db3', N'en-US', N'Ground', N'Ground', N'commerce.stepwiseshipping', 0, 1, N'20af06d8-88f6-4e21-ae0f-31f413553480', 1, CAST(0x0000A11901138649 AS DateTime), CAST(0x0000A1190113B409 AS DateTime))
INSERT [dbo].[ShippingMethod] ([ShippingMethodId], [LanguageId], [ShippingMethodName], [Description], [ShippingCostCalculator], [ConfiguredMode], [Enabled], [GroupId], [IsDefault], [Created], [LastModified]) VALUES (N'2289df5d-5715-435e-a2c2-4717ea474be5', N'en-US', N'Email delivery', N'Email delivery', N'commerce.stepwiseshipping', 0, 1, N'aa65f874-74dc-4ae0-901e-9246ce03285f', 1, CAST(0x0000A11901138649 AS DateTime), CAST(0x0000A1190113B409 AS DateTime))

INSERT [dbo].[ShippingRates] ([ShippingGroupId], [MaxLimit], [Price]) VALUES (N'c0053cda-10bd-4329-8f24-edef93b92a88', -1, 2.0000)
INSERT [dbo].[ShippingRates] ([ShippingGroupId], [MaxLimit], [Price]) VALUES (N'1bed28d1-e239-4432-8f53-dd90756b6d1d', -1, 5.0000)
INSERT [dbo].[ShippingRates] ([ShippingGroupId], [MaxLimit], [Price]) VALUES (N'00d38097-eea6-41b3-81df-d27a030cb816', -1, 10.0000)
INSERT [dbo].[ShippingRates] ([ShippingGroupId], [MaxLimit], [Price]) VALUES (N'20af06d8-88f6-4e21-ae0f-31f413553480', -1, 15.0000)
INSERT [dbo].[ShippingRates] ([ShippingGroupId], [MaxLimit], [Price]) VALUES (N'aa65f874-74dc-4ae0-901e-9246ce03285f', -1, 0.0000)
GO