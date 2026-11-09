/*
    Court Registry - put the removal wording directly into the email templates.

    RUN THIS AGAINST: the jud12.flcourts.org database (the one holding the other
    tjc_car_* tables).

    The [Heading], [Caveat] and [ReasonBlock] placeholders used to pull their text
    from the module's code, which meant the wording could only be changed by a
    developer. The text is now written into the templates themselves, so court
    administration can edit it on the Email Templates screen.

    The reason paragraph is wrapped in [if:Reason] ... [/if] so it is left out when
    the attorney gave no reason, rather than printing a heading with nothing under it.

    Only rows that have never been edited are updated (DateModified IS NULL), so
    wording someone has already changed by hand is left alone. It prints what it did,
    and the check at the end makes a bad write obvious.
*/

SET NOCOUNT ON
GO

IF EXISTS (SELECT 1 FROM dbo.tjc_car_email_templates WHERE TemplateKey = N'withdrawal.submitted' AND DateModified IS NULL)
BEGIN
    UPDATE dbo.tjc_car_email_templates
    SET Body = N'<p>Dear [FullName],</p>
<p>Court Administration has received your request to be removed from the 12<sup>th</sup> Circuit court appointed registry. The request is pending review. You will receive another email when it has been acted on, and you remain on the registry until then.</p>
<p><strong>Removal applies to future appointments only</strong><br />Removal from the registry applies to future appointments only. You are required to continue representation on every case you are currently appointed to until either the case ends or you file a motion to withdraw in that case, set the motion for hearing, and the motion is granted by the court. Your representation on a case you are already appointed to is not over by this request to be removed from the registry.</p>
[if:Reason]<p><strong>Reason you gave:</strong><br />[Reason]</p>[/if]
<p>Requested on [DateRequested] and signed [Signature].</p>'
    WHERE TemplateKey = N'withdrawal.submitted' AND DateModified IS NULL
    PRINT 'Updated withdrawal.submitted.'
END
ELSE
    PRINT 'Skipped withdrawal.submitted - either missing, or it has been edited by hand.'
GO

IF EXISTS (SELECT 1 FROM dbo.tjc_car_email_templates WHERE TemplateKey = N'withdrawal.approved' AND DateModified IS NULL)
BEGIN
    UPDATE dbo.tjc_car_email_templates
    SET Body = N'<p>Dear [FullName],</p>
<p>Court Administration has approved your request to be removed from the 12<sup>th</sup> Circuit court appointed registry, effective [EffectiveDate]. You will not be considered for new appointments.</p>
<p><strong>Removal from the registry applies to future appointments only. You are required to continue representation on every case you are currently appointed to until either the case ends or you file a motion to withdraw in that case, set the motion for hearing, and the motion is granted by the court. Your representation on a case you are already appointed to is not over by this request to be removed from the registry.</strong></p>
[NoteBlock]'
    WHERE TemplateKey = N'withdrawal.approved' AND DateModified IS NULL
    PRINT 'Updated withdrawal.approved.'
END
ELSE
    PRINT 'Skipped withdrawal.approved - either missing, or it has been edited by hand.'
GO

SELECT TemplateKey,
       LEN(Body) AS BodyChars,
       LEFT(Body, 28) AS BodyStartsWith,
       CASE WHEN Body LIKE N'%[[]Caveat]%' OR Body LIKE N'%[[]Heading]%' OR Body LIKE N'%[[]ReasonBlock]%'
                 THEN 'STILL HAS PLACEHOLDERS'
            WHEN Body NOT LIKE N'<p>%' THEN 'BAD - does not start with a paragraph'
            ELSE 'ok' END AS BodyCheck
FROM dbo.tjc_car_email_templates ORDER BY TemplateKey
GO
