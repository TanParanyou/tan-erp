//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_survey_evidence_request.g.dart';

/// UpdateSurveyEvidenceRequest
///
/// Properties:
/// * [fileId] 
/// * [kind] 
/// * [caption] 
/// * [sortOrder] 
@BuiltValue()
abstract class UpdateSurveyEvidenceRequest implements Built<UpdateSurveyEvidenceRequest, UpdateSurveyEvidenceRequestBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  UpdateSurveyEvidenceRequest._();

  factory UpdateSurveyEvidenceRequest([void updates(UpdateSurveyEvidenceRequestBuilder b)]) = _$UpdateSurveyEvidenceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSurveyEvidenceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSurveyEvidenceRequest> get serializer => _$UpdateSurveyEvidenceRequestSerializer();
}

class _$UpdateSurveyEvidenceRequestSerializer implements PrimitiveSerializer<UpdateSurveyEvidenceRequest> {
  @override
  final Iterable<Type> types = const [UpdateSurveyEvidenceRequest, _$UpdateSurveyEvidenceRequest];

  @override
  final String wireName = r'UpdateSurveyEvidenceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSurveyEvidenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateSurveyEvidenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSurveyEvidenceRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.caption = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateSurveyEvidenceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSurveyEvidenceRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


