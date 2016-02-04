CREATE TABLE [dbo].[CreditCards] (
    [u_id]                           NVARCHAR (50)  NOT NULL,
    [u_payment_group_id]             NVARCHAR (50)  NOT NULL,
    [u_cc_number]                    NVARCHAR (512) NOT NULL,
    [i_expiration_month]             INT            NOT NULL,
    [i_expiration_year]              INT            NOT NULL,
    [u_last_4_digits]                NVARCHAR (10)  NOT NULL,
    [u_billing_address]              NVARCHAR (50)  NOT NULL,
    [i_keyindex]                     INT            NULL,
    [u_user_id_changed_by]           NVARCHAR (50)  NULL,
    [dt_date_last_changed]           DATETIME       DEFAULT (getdate()) NULL,
    [dt_csadapter_date_last_changed] DATETIME       NULL,
    [dt_date_created]                DATETIME       DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CreditCards] PRIMARY KEY CLUSTERED ([u_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Idx_CreditCards_csadapterdatelastchanged]
    ON [dbo].[CreditCards]([dt_csadapter_date_last_changed] ASC);

