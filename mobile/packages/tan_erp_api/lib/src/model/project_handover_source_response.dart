//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_handover_source_response.g.dart';

/// ProjectHandoverSourceResponse
///
/// Properties:
/// * [quotationId] 
/// * [quotationNumber] 
/// * [quotationStatus] 
/// * [quotationRowVersion] 
/// * [contractAmount] 
/// * [opportunityStage] 
/// * [existingProjectId] 
/// * [existingProjectCode] 
@BuiltValue()
abstract class ProjectHandoverSourceResponse implements Built<ProjectHandoverSourceResponse, ProjectHandoverSourceResponseBuilder> {
  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'quotationNumber')
  String? get quotationNumber;

  @BuiltValueField(wireName: r'quotationStatus')
  String? get quotationStatus;

  @BuiltValueField(wireName: r'quotationRowVersion')
  String? get quotationRowVersion;

  @BuiltValueField(wireName: r'contractAmount')
  double? get contractAmount;

  @BuiltValueField(wireName: r'opportunityStage')
  String? get opportunityStage;

  @BuiltValueField(wireName: r'existingProjectId')
  String? get existingProjectId;

  @BuiltValueField(wireName: r'existingProjectCode')
  String? get existingProjectCode;

  ProjectHandoverSourceResponse._();

  factory ProjectHandoverSourceResponse([void updates(ProjectHandoverSourceResponseBuilder b)]) = _$ProjectHandoverSourceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectHandoverSourceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectHandoverSourceResponse> get serializer => _$ProjectHandoverSourceResponseSerializer();
}

class _$ProjectHandoverSourceResponseSerializer implements PrimitiveSerializer<ProjectHandoverSourceResponse> {
  @override
  final Iterable<Type> types = const [ProjectHandoverSourceResponse, _$ProjectHandoverSourceResponse];

  @override
  final String wireName = r'ProjectHandoverSourceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectHandoverSourceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quotationNumber != null) {
      yield r'quotationNumber';
      yield serializers.serialize(
        object.quotationNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quotationStatus != null) {
      yield r'quotationStatus';
      yield serializers.serialize(
        object.quotationStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quotationRowVersion != null) {
      yield r'quotationRowVersion';
      yield serializers.serialize(
        object.quotationRowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.contractAmount != null) {
      yield r'contractAmount';
      yield serializers.serialize(
        object.contractAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.opportunityStage != null) {
      yield r'opportunityStage';
      yield serializers.serialize(
        object.opportunityStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.existingProjectId != null) {
      yield r'existingProjectId';
      yield serializers.serialize(
        object.existingProjectId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.existingProjectCode != null) {
      yield r'existingProjectCode';
      yield serializers.serialize(
        object.existingProjectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectHandoverSourceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectHandoverSourceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'quotationNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationNumber = valueDes;
          break;
        case r'quotationStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationStatus = valueDes;
          break;
        case r'quotationRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationRowVersion = valueDes;
          break;
        case r'contractAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractAmount = valueDes;
          break;
        case r'opportunityStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityStage = valueDes;
          break;
        case r'existingProjectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.existingProjectId = valueDes;
          break;
        case r'existingProjectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.existingProjectCode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectHandoverSourceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectHandoverSourceResponseBuilder();
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


