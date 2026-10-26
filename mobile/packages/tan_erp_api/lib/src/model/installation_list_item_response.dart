//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'installation_list_item_response.g.dart';

/// InstallationListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
/// * [projectCode] 
/// * [projectName] 
/// * [scheduledStart] 
/// * [scheduledEnd] 
/// * [checklistDone] 
/// * [checklistTotal] 
/// * [openDefects] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class InstallationListItemResponse implements Built<InstallationListItemResponse, InstallationListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'projectName')
  String? get projectName;

  @BuiltValueField(wireName: r'scheduledStart')
  Date? get scheduledStart;

  @BuiltValueField(wireName: r'scheduledEnd')
  Date? get scheduledEnd;

  @BuiltValueField(wireName: r'checklistDone')
  int? get checklistDone;

  @BuiltValueField(wireName: r'checklistTotal')
  int? get checklistTotal;

  @BuiltValueField(wireName: r'openDefects')
  int? get openDefects;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  InstallationListItemResponse._();

  factory InstallationListItemResponse([void updates(InstallationListItemResponseBuilder b)]) = _$InstallationListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InstallationListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InstallationListItemResponse> get serializer => _$InstallationListItemResponseSerializer();
}

class _$InstallationListItemResponseSerializer implements PrimitiveSerializer<InstallationListItemResponse> {
  @override
  final Iterable<Type> types = const [InstallationListItemResponse, _$InstallationListItemResponse];

  @override
  final String wireName = r'InstallationListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InstallationListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectName != null) {
      yield r'projectName';
      yield serializers.serialize(
        object.projectName,
        specifiedType: const FullType.nullable(String),
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
    if (object.checklistDone != null) {
      yield r'checklistDone';
      yield serializers.serialize(
        object.checklistDone,
        specifiedType: const FullType(int),
      );
    }
    if (object.checklistTotal != null) {
      yield r'checklistTotal';
      yield serializers.serialize(
        object.checklistTotal,
        specifiedType: const FullType(int),
      );
    }
    if (object.openDefects != null) {
      yield r'openDefects';
      yield serializers.serialize(
        object.openDefects,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InstallationListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InstallationListItemResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'projectName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectName = valueDes;
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
        case r'checklistDone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.checklistDone = valueDes;
          break;
        case r'checklistTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.checklistTotal = valueDes;
          break;
        case r'openDefects':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.openDefects = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InstallationListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InstallationListItemResponseBuilder();
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


