//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/checklist_item_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'installation_request.g.dart';

/// InstallationRequest
///
/// Properties:
/// * [projectId] 
/// * [scheduledStart] 
/// * [scheduledEnd] 
/// * [crewName] 
/// * [note] 
/// * [checklist] 
@BuiltValue()
abstract class InstallationRequest implements Built<InstallationRequest, InstallationRequestBuilder> {
  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'scheduledStart')
  Date? get scheduledStart;

  @BuiltValueField(wireName: r'scheduledEnd')
  Date? get scheduledEnd;

  @BuiltValueField(wireName: r'crewName')
  String? get crewName;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'checklist')
  BuiltList<ChecklistItemRequest>? get checklist;

  InstallationRequest._();

  factory InstallationRequest([void updates(InstallationRequestBuilder b)]) = _$InstallationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InstallationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InstallationRequest> get serializer => _$InstallationRequestSerializer();
}

class _$InstallationRequestSerializer implements PrimitiveSerializer<InstallationRequest> {
  @override
  final Iterable<Type> types = const [InstallationRequest, _$InstallationRequest];

  @override
  final String wireName = r'InstallationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InstallationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType(String),
      );
    }
    if (object.scheduledStart != null) {
      yield r'scheduledStart';
      yield serializers.serialize(
        object.scheduledStart,
        specifiedType: const FullType(Date),
      );
    }
    if (object.scheduledEnd != null) {
      yield r'scheduledEnd';
      yield serializers.serialize(
        object.scheduledEnd,
        specifiedType: const FullType(Date),
      );
    }
    if (object.crewName != null) {
      yield r'crewName';
      yield serializers.serialize(
        object.crewName,
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
    if (object.checklist != null) {
      yield r'checklist';
      yield serializers.serialize(
        object.checklist,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ChecklistItemRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InstallationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InstallationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'scheduledStart':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.scheduledStart = valueDes;
          break;
        case r'scheduledEnd':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.scheduledEnd = valueDes;
          break;
        case r'crewName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.crewName = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'checklist':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ChecklistItemRequest)]),
          ) as BuiltList<ChecklistItemRequest>?;
          if (valueDes == null) continue;
          result.checklist.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InstallationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InstallationRequestBuilder();
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


