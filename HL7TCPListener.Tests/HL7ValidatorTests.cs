using Xunit;
using HL7TCPListener;
using System.IO;
using NHapi.Base.Parser;
using NHapi.Base.Model;

public class HL7ValidatorTests
{
    [Fact]
    public void Validate_ShouldFail_OnEmptyMessage()
    {
        var schemaJson = "{ \"Versions\": { \"2.5\": { \"Messages\": {} } } }";
        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_test.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        var (isValid, error) = validator.Validate(null!);

        Assert.False(isValid);
        Assert.Contains("null", error, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ShouldPass_OnValid_ADT_A01_Message()
    {
        // Arrange
        var schemaJson = """
        {
          "Versions": {
            "2.5": {
              "Messages": {
                "ADT^A01": {
                  "Description": "Patient Admit",
                  "Segments": {
                    "MSH": {
                      "Required": true,
                      "Fields": {
                        "FieldSeparator": { "Position": 1, "Required": true },
                        "EncodingCharacters": { "Position": 2, "Required": true },
                        "SendingApplication": { "Position": 3, "Required": true },
                        "MessageType": { "Position": 9, "Required": true }
                      }
                    },
                    "PID": {
                      "Required": true,
                      "Fields": {
                        "PatientID": { "Position": 3, "Required": true },
                        "PatientName": { "Position": 5, "Required": true }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_test_valid.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        // This is a minimal, valid ADT^A01 message (enough for NHapi to parse)
        string hl7 = "MSH|^~\\&|SendingApp|Facility|ReceivingApp|Facility|20250101120000||ADT^A01|MSG00001|P|2.5\r"
                   + "PID|1||12345^^^Hospital^MR||Doe^John\r";

        var parser = new PipeParser();
        IMessage message = parser.Parse(hl7);

        // Act
        var (isValid, error) = validator.Validate(message);

        // Assert
        Assert.True(isValid);
        Assert.True(string.IsNullOrEmpty(error));
    }

	[Fact]
    public void Validate_ShouldFail_When_PID_Missing()
    {
        // Arrange
        var schemaJson = """
        {
          "Versions": {
            "2.5": {
              "Messages": {
                "ADT^A01": {
                  "Description": "Patient Admit",
                  "Segments": {
                    "MSH": {
                      "Required": true,
                      "Fields": {
                        "FieldSeparator": { "Position": 1, "Required": true },
                        "EncodingCharacters": { "Position": 2, "Required": true },
                        "SendingApplication": { "Position": 3, "Required": true },
                        "MessageType": { "Position": 9, "Required": true }
                      }
                    },
                    "PID": {
                      "Required": true,
                      "Fields": {
                        "PatientID": { "Position": 3, "Required": true },
                        "PatientName": { "Position": 5, "Required": true }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_test_missing_pid.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        // This HL7 message is missing the PID segment
        string hl7 = "MSH|^~\\&|SendingApp|Facility|ReceivingApp|Facility|20250101120000||ADT^A01|MSG00001|P|2.5\r";

        var parser = new NHapi.Base.Parser.PipeParser();
        var message = parser.Parse(hl7);

        // Act
        var (isValid, error) = validator.Validate(message);

        // Assert
        Assert.False(isValid);
        Assert.Contains("PID", error);
        Assert.Contains("Missing required segment", error);
    }

	    [Fact]
    public void Validate_ShouldFail_When_Required_Field_Missing()
    {
        // Arrange
        var schemaJson = """
        {
          "Versions": {
            "2.5": {
              "Messages": {
                "ADT^A01": {
                  "Description": "Patient Admit",
                  "Segments": {
                    "MSH": {
                      "Required": true,
                      "Fields": {
                        "FieldSeparator": { "Position": 1, "Required": true },
                        "EncodingCharacters": { "Position": 2, "Required": true },
                        "SendingApplication": { "Position": 3, "Required": true },
                        "MessageType": { "Position": 9, "Required": true }
                      }
                    },
                    "PID": {
                      "Required": true,
                      "Fields": {
                        "PatientID": { "Position": 3, "Required": true },
                        "PatientName": { "Position": 5, "Required": true }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_test_missing_field.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        // This HL7 message is missing PID-5 (Patient Name)
        string hl7 = "MSH|^~\\&|SendingApp|Facility|ReceivingApp|Facility|20250101120000||ADT^A01|MSG00001|P|2.5\r"
                   + "PID|1||12345^^^Hospital^MR||\r"; // <-- Missing patient name here!

        var parser = new NHapi.Base.Parser.PipeParser();
        var message = parser.Parse(hl7);

        // Act
        var (isValid, error) = validator.Validate(message);

        // Assert
        Assert.False(isValid);
        Assert.Contains("PID-5", error);
        Assert.Contains("Missing required field", error);
    }

	    [Fact]
    public void Validate_ShouldFail_When_Schema_Not_Found()
    {
        // Arrange
        var schemaJson = """
        {
          "Versions": {
            "2.5": {
              "Messages": {
                "ADT^A01": {
                  "Description": "Patient Admit",
                  "Segments": {
                    "MSH": {
                      "Required": true,
                      "Fields": {
                        "FieldSeparator": { "Position": 1, "Required": true },
                        "EncodingCharacters": { "Position": 2, "Required": true },
                        "SendingApplication": { "Position": 3, "Required": true },
                        "MessageType": { "Position": 9, "Required": true }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_test_no_orm.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        // This HL7 message uses ORM^O01, but the schema only defines ADT^A01
        string hl7 = "MSH|^~\\&|SendingApp|Facility|ReceivingApp|Facility|20250101120000||ORM^O01|MSG99999|P|2.5\r"
                   + "PID|1||12345^^^Hospital^MR||Doe^John\r";

        var parser = new NHapi.Base.Parser.PipeParser();
        var message = parser.Parse(hl7);

        // Act
        var (isValid, error) = validator.Validate(message);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Schema not found", error, System.StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ORM^O01", error);
    }

	    [Fact]
    public void Validate_ShouldPass_When_Optional_Segment_Missing()
    {
        // Arrange
        var schemaJson = """
        {
          "Versions": {
            "2.5": {
              "Messages": {
                "ADT^A01": {
                  "Description": "Patient Admit",
                  "Segments": {
                    "MSH": {
                      "Required": true,
                      "Fields": {
                        "FieldSeparator": { "Position": 1, "Required": true },
                        "EncodingCharacters": { "Position": 2, "Required": true },
                        "MessageType": { "Position": 9, "Required": true }
                      }
                    },
                    "PID": {
                      "Required": true,
                      "Fields": {
                        "PatientID": { "Position": 3, "Required": true }
                      }
                    },
                    "PV1": {
                      "Required": false,
                      "Fields": {
                        "VisitNumber": { "Position": 2, "Required": false }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_optional_segment.json");
        File.WriteAllText(tempSchemaPath, schemaJson);

        var schema = HL7Schema.Load(tempSchemaPath);
        var logger = new UILogger();
        var validator = new HL7Validator(schema, logger);

        // Message has no PV1 segment (which is optional)
        string hl7 = "MSH|^~\\&|App|Fac|Dest|Fac|20250101120000||ADT^A01|MSG001|P|2.5\r"
                   + "PID|1||12345^^^Hosp^MR||Doe^John\r";

        var parser = new NHapi.Base.Parser.PipeParser();
        var message = parser.Parse(hl7);

        // Act
        var (isValid, error) = validator.Validate(message);

        // Assert
        Assert.True(isValid);
        Assert.True(string.IsNullOrWhiteSpace(error));
    }

	[Fact]
	public void Validate_ShouldUse_Default_Version_When_Missing()
	{
		// Arrange
		var schemaJson = """
		{
		"Versions": {
			"2.5": {
			"Messages": {
				"ADT^A01": {
				"Description": "Patient Admit",
				"Segments": {
					"MSH": { "Required": true, "Fields": {} }
				}
				}
			}
			}
		}
		}
		""";

		string tempSchemaPath = Path.Combine(Path.GetTempPath(), "schema_default_version.json");
		File.WriteAllText(tempSchemaPath, schemaJson);

		var schema = HL7Schema.Load(tempSchemaPath);
		var logger = new UILogger();
		var validator = new HL7Validator(schema, logger);

		// NOTE: MSH-12 contains a placeholder value that NHapi requires
		string hl7 = "MSH|^~\\&|App|Fac|Dest|Fac|20250101120000||ADT^A01|MSG12345|P|2.5|\r";

		var parser = new NHapi.Base.Parser.PipeParser();
		var message = parser.Parse(hl7);

		// Manually clear MSH-12 to simulate missing version
		var msh = (NHapi.Model.V25.Segment.MSH)message.GetStructure("MSH");
		msh.VersionID.VersionID.Value = null;

		// Act
		var (isValid, error) = validator.Validate(message);

		// Assert
		Assert.True(isValid);
		Assert.True(string.IsNullOrEmpty(error));
	}

}