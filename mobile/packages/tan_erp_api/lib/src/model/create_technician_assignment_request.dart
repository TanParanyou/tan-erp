//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_technician_assignment_request.g.dart';

/// CreateTechnicianAssignmentRequest
///
/// Properties:
/// * [technicianUserId] 
/// * [workDate] 
/// * [assignmentKind] 
/// * [installationJobId] 
/// * [serviceRequestId] 
/// * [note] 
/// * [allowConflict] 
@BuiltValue()
abstract class CreateTechnicianAssignmentRequest implements Built<CreateTechnicianAssignmentRequest, CreateTechnicianAssignmentRequestBuilder> {
  @BuiltValueField(wireName: r'technicianUserId')
  String? get technicianUserId;

  @BuiltValueField(wireName: r'workDate')
  Date? get workDate;

  @BuiltValueField(wireName: r'assignmentKind')
  String? get assignmentKind;

  @BuiltValueField(wireName: r'installationJobId')
  String? get installationJobId;

  @BuiltValueField(wireName: r'serviceRequestId')
  String? get serviceRequestId;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'allowConflict')
  bool? get allowConflict;

  CreateTechnicianAssignmentRequest._();

  factory CreateTechnicianAssignmentRequest([void updates(CreateTechnicianAssignmentRequestBuilder b)]) = _$CreateTechnicianAssignmentRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateTechnicianAssignmentRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateTechnicianAssignmentRequest> get serializer => _$CreateTechnicianAssignmentRequestSerializer();
}

class _$CreateTechnicianAssignmentRequestSerializer implements PrimitiveSerializer<CreateTechnicianAssignmentRequest> {
  @override
  final Iterable<Type> types = const [CreateTechnicianAssignmentRequest, _$CreateTechnicianAssignmentRequest];

  @override
  final String wireName = r'CreateTechnicianAssignmentRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateTechnicianAssignmentRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.technicianUserId != null) {
      yield r'technicianUserId';
      yield serializers.serialize(
        object.technicianUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.workDate != null) {
      yield r'workDate';
      yield serializers.serialize(
        object.workDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.assignmentKind != null) {
      yield r'assignmentKind';
      yield serializers.serialize(
        object.assignmentKind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.installationJobId != null) {
      yield r'installationJobId';
      yield serializers.serialize(
        object.installationJobId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.serviceRequestId != null) {
      yield r'serviceRequestId';
      yield serializers.serialize(
        object.serviceRequestId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.allowConflict != null) {
      yield r'allowConflict';
      yield serializers.serialize(
        object.allowConflict,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateTechnicianAssignmentRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateTechnicianAssignmentRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'technicianUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.technicianUserId = valueDes;
          break;
        case r'workDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.workDate = valueDes;
          break;
        case r'assignmentKind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.assignmentKind = valueDes;
          break;
        case r'installationJobId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.installationJobId = valueDes;
          break;
        case r'serviceRequestId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.serviceRequestId = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'allowConflict':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.allowConflict = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateTechnicianAssignmentRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateTechnicianAssignmentRequestBuilder();
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


