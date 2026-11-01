//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'technician_assignment_response.g.dart';

/// TechnicianAssignmentResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [technician] 
/// * [workDate] 
/// * [assignmentKind] 
/// * [installationJobId] 
/// * [installationJobNumber] 
/// * [serviceRequestId] 
/// * [serviceRequestNumber] 
/// * [note] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [hasConflict] 
@BuiltValue()
abstract class TechnicianAssignmentResponse implements Built<TechnicianAssignmentResponse, TechnicianAssignmentResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'technician')
  ServicePersonResponse? get technician;

  @BuiltValueField(wireName: r'workDate')
  Date? get workDate;

  @BuiltValueField(wireName: r'assignmentKind')
  String? get assignmentKind;

  @BuiltValueField(wireName: r'installationJobId')
  String? get installationJobId;

  @BuiltValueField(wireName: r'installationJobNumber')
  String? get installationJobNumber;

  @BuiltValueField(wireName: r'serviceRequestId')
  String? get serviceRequestId;

  @BuiltValueField(wireName: r'serviceRequestNumber')
  String? get serviceRequestNumber;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'createdBy')
  ServicePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'hasConflict')
  bool? get hasConflict;

  TechnicianAssignmentResponse._();

  factory TechnicianAssignmentResponse([void updates(TechnicianAssignmentResponseBuilder b)]) = _$TechnicianAssignmentResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TechnicianAssignmentResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TechnicianAssignmentResponse> get serializer => _$TechnicianAssignmentResponseSerializer();
}

class _$TechnicianAssignmentResponseSerializer implements PrimitiveSerializer<TechnicianAssignmentResponse> {
  @override
  final Iterable<Type> types = const [TechnicianAssignmentResponse, _$TechnicianAssignmentResponse];

  @override
  final String wireName = r'TechnicianAssignmentResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TechnicianAssignmentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.technician != null) {
      yield r'technician';
      yield serializers.serialize(
        object.technician,
        specifiedType: const FullType(ServicePersonResponse),
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
    if (object.installationJobNumber != null) {
      yield r'installationJobNumber';
      yield serializers.serialize(
        object.installationJobNumber,
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
    if (object.serviceRequestNumber != null) {
      yield r'serviceRequestNumber';
      yield serializers.serialize(
        object.serviceRequestNumber,
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
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.hasConflict != null) {
      yield r'hasConflict';
      yield serializers.serialize(
        object.hasConflict,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    TechnicianAssignmentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TechnicianAssignmentResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'technician':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.technician.replace(valueDes);
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
        case r'installationJobNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.installationJobNumber = valueDes;
          break;
        case r'serviceRequestId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.serviceRequestId = valueDes;
          break;
        case r'serviceRequestNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.serviceRequestNumber = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'hasConflict':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasConflict = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  TechnicianAssignmentResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TechnicianAssignmentResponseBuilder();
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


