CREATE TABLE [dbo].[UserObject] (
    [u_user_id]                             NVARCHAR (50)  NOT NULL,
    [u_org_id]                              NVARCHAR (50)  NULL,
    [u_user_type]                           NVARCHAR (50)  NULL,
    [u_first_name]                          NVARCHAR (50)  NULL,
    [u_last_name]                           NVARCHAR (50)  NULL,
    [u_email_address]                       NVARCHAR (64)  NULL,
    [u_preferred_address]                   NVARCHAR (50)  NULL,
    [u_addresses]                           NVARCHAR (255) NULL,
    [u_preferred_credit_card]               NVARCHAR (50)  NULL,
    [u_credit_cards]                        NVARCHAR (255) NULL,
    [u_tel_number]                          NVARCHAR (32)  NULL,
    [u_tel_extension]                       NVARCHAR (50)  NULL,
    [u_fax_number]                          NVARCHAR (32)  NULL,
    [u_fax_extension]                       NVARCHAR (50)  NULL,
    [u_user_security_password]              NVARCHAR (100) NULL,
    [u_user_id_changed_by]                  NVARCHAR (50)  NULL,
    [u_account_status]                      NVARCHAR (50)  NULL,
    [u_user_catalog_set]                    NVARCHAR (50)  NULL,
    [dt_date_registered]                    DATETIME       NULL,
    [u_campaign_history]                    NVARCHAR (50)  NULL,
    [dt_date_last_changed]                  DATETIME       CONSTRAINT [DF__UserObjec__d_dat__1A69E950] DEFAULT (getdate()) NULL,
    [dt_csadapter_date_last_changed]        DATETIME       NULL,
    [dt_date_created]                       DATETIME       CONSTRAINT [DF__UserObjec__d_dat__1B5E0D89] DEFAULT (getdate()) NULL,
    [u_language]                            NVARCHAR (128) NULL,
    [u_Pref1]                               NVARCHAR (50)  NULL,
    [u_Pref2]                               NVARCHAR (50)  NULL,
    [u_Pref3]                               NVARCHAR (50)  NULL,
    [u_Pref4]                               NVARCHAR (50)  NULL,
    [u_Pref5]                               NVARCHAR (50)  NULL,
    [u_password_question]                   NVARCHAR (255) NULL,
    [u_password_answer]                     NVARCHAR (255) NULL,
    [u_logon_error_dates]                   NVARCHAR (255) NULL,
    [u_password_answer_error_dates]         NVARCHAR (255) NULL,
    [i_keyindex]                            INT            NULL,
    [i_access_level_id]                     INT            NULL,
    [b_change_password]                     BIT            CONSTRAINT [DF_UserObject_b_change_password] DEFAULT ((0)) NULL,
    [dt_date_last_password_changed]         DATETIME       CONSTRAINT [DF_UserObject_dt_date_last_password_changed ] DEFAULT (getdate()) NULL,
    [dt_last_logon]                         DATETIME       NULL,
    [dt_last_lockedout_date]                DATETIME       NULL,
    [u_application_name]                    NVARCHAR (256) NULL,
    [dt_last_activity_date]                 DATETIME       NULL,
    [b_direct_mail_opt_out]                 BIT            CONSTRAINT [DF_UserObject_b_direct_mail_opt_out] DEFAULT ((0)) NULL,
    [b_express_checkout]                    BIT            CONSTRAINT [DF_UserObject_b_express_checkout] DEFAULT ((0)) NULL,
    [dt_date_address_list_last_changed]     DATETIME       NULL,
    [dt_date_credit_card_list_last_changed] DATETIME       NULL,
    [u_preferred_shipping_method]           NVARCHAR (50)  NULL,
    [u_default_shopper_list]                NVARCHAR (50)  NULL,
    [u_external_id]                         NVARCHAR (256) NOT NULL,
    [u_comment]                             NVARCHAR (256) NULL,
    CONSTRAINT [PK_UserObject] PRIMARY KEY CLUSTERED ([u_user_id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserObject_ExternalId]
    ON [dbo].[UserObject]([u_external_id] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_UserObject_firstname]
    ON [dbo].[UserObject]([u_first_name] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_UserObject_lastname]
    ON [dbo].[UserObject]([u_last_name] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_UserObject_datecreated]
    ON [dbo].[UserObject]([dt_date_created] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_UserObject_datelastchanged]
    ON [dbo].[UserObject]([dt_date_last_changed] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_UserObject_csadapterdatelastchanged]
    ON [dbo].[UserObject]([dt_csadapter_date_last_changed] ASC);

